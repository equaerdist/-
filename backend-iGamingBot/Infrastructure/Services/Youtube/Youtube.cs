using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class Youtube : IYoutube
    {
        private List<string> _htmlContents = new();
        private readonly HttpClient _client;
        private readonly ILogger<Youtube> _logger;
        private static string _metaObject = "var ytInitialData = ";
        private static string _playerMetaObject = "var ytInitialPlayerResponse = ";
        private static string _userAgent = "Mozilla/5.0 (Linux; Android 6.0; " +
            "Nexus 5 Build/MRA58N) " +
            "AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/126.0.0.0 Mobile " +
            "Safari/537.36";
        private static string _streamCondition = "videoId";

        public Youtube(HttpClient client, ILogger<Youtube> logger)
        {
            _client = client;
            _logger = logger;
            SetupClient();
        }
        private void SetupClient()
        {
            _client.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
        }
        private string? GetScriptContentWith(string html, string prop)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);

            var scriptTags = document.QuerySelectorAll("script");

            foreach (var script in scriptTags)
            {
                var scriptContent = script.TextContent;
                if (scriptContent.Contains(prop))
                {
                    if (scriptContent.Length <= prop.Length + "null;".Length)
                        continue;
                    return scriptContent;
                }
            }
            return null;
        }
        private string? GetYtInitialData(string html) => GetScriptContentWith(html, _metaObject);
        private string? GetPlayerInitialData(string html) => GetScriptContentWith(html, _playerMetaObject);
        private static string EnsureUrlHasProtocol(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
            }

            return url;
        }
        private async Task CheckAboutAgreement()
        {
            var values = new Dictionary<string, string>
        {
            { "gl", "PL" },
            { "m", "1" },
            { "app", "0" },
            { "pc", "yt" },
            { "continue", "https://m.youtube.com/@LofiGirl?cbrd=1" },
            { "x", "6" },
            { "bl", "boq_identityfrontenduiserver_20240716.08_p1" },
            { "hl", "pl" },
            { "src", "1" },
            { "cm", "2" },
            { "set_ytc", "true" },
            { "set_apyt", "true" },
            { "set_eom", "false" }
        };

            var content = new FormUrlEncodedContent(values);

            var response = await _client.PostAsync("https://consent.youtube.com/save", content);

            var responseString = await response.Content.ReadAsStringAsync();
        }
        public async Task<string> GetUserIdentifierByLinkAsync(string link)
        {
            link = EnsureUrlHasProtocol(link);
            try
            {
                int tries = 0;
                Start:
                _logger.LogDebug($"$Начинаю парсинг externalId для {link}");
                var response = await _client.GetAsync(link);
                response.EnsureSuccessStatusCode();
                var htmlContent = await response.Content.ReadAsStringAsync();
                var ytInitialInfo = GetYtInitialData(htmlContent);
                if (ytInitialInfo is null)
                {
                    _logger.LogError($"Не удалось обнаружить externalId для канала {link}");
                    if(tries > 1)
                        throw new AppException(AppDictionary.YoutubeIdentifierNotFound);
                    await CheckAboutAgreement();
                    tries++;
                    goto Start;
                }
                var externalId = GetPropertyValue("externalId", ytInitialInfo);
                if (externalId is null)
                    throw new AppException(AppDictionary.YoutubeIdentifierNotFound);
                return externalId;

            }
            catch (HttpRequestException e)
            {
                _logger.LogError($"Ошибка при запросе для {link}: \n{e.Message}");
                throw;
            }
        }
        private string? GetPropertyValue(string property, string content)
        {
            var prestart = content.IndexOf(property);
            if (prestart == -1)
                return null;
            var externalContent = content.Substring(prestart);
            var start = externalContent.IndexOf(":") + 1;
            var end = externalContent.IndexOf(",");
            if (end == -1)
                return null;
            var length = end - start;
            var externalId = externalContent.Substring(start, length).Replace(@"\x22", "");
            return externalId;
        }
        public string ConstructYouTubeUrl(string username)
        {
            if (string.IsNullOrEmpty(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));
            return $"https://www.youtube.com/@{username}";
        }

        public  string ExtractYouTubeUsernameFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            Uri uri = new Uri(url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : "http://" + url);
            if (uri.Host != "www.youtube.com" && uri.Host != "youtube.com")
                throw new ArgumentException("URL is not a valid YouTube URL", nameof(url));

            string path = uri.AbsolutePath;
            if (string.IsNullOrEmpty(path) || !path.StartsWith("/@"))
                throw new ArgumentException("URL does not contain a valid YouTube username", nameof(url));

            return path.Substring(2).Trim('/');
        }

        public async Task<StreamInformation> UserIsStreaming(string channelId)
        {
            _logger.LogDebug($"Начинаю проверку на стриминг для {channelId}");
            var url = $"https://youtube.com/channel/{channelId}/live";
            try
            {
                var response = await _client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var htmlContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine(htmlContent);
                if (_htmlContents.Count > 5)
                    _htmlContents = _htmlContents.Take(5).ToList();
                _htmlContents.Add(htmlContent);
                var ytPlayerInitalData = GetPlayerInitialData(htmlContent);
                if (ytPlayerInitalData is null)
                {
                    _logger.LogError("Не удалось найти объект инициалзиации\n Будем считать, что стрима нет");
                    return new() { IsLive = false, Link = null };
                }
              
                var videoId = GetPropertyValue(_streamCondition, ytPlayerInitalData)?.Replace("\"", "");
                return new() { IsLive = videoId != null, Link = $"https://www.youtube.com/watch?v={videoId}" };
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Произошла ошибка при запросе {channelId}: \n{e.Message}");
                throw;
            }
        }

        public List<string> GetLiveHtmls()
        {
            return _htmlContents;
        }
    }
}
