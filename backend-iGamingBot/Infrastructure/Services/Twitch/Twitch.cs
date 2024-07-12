﻿
using System.Linq;
using System.Text.Json.Serialization;
using TwitchLib.Api;

namespace backend_iGamingBot.Infrastructure.Services.Twitch
{
    public record AccessTokenResponse(
     [property: JsonPropertyName("access_token")] string AccessToken,
     [property: JsonPropertyName("expires_in")] string ExpiresIn,
     [property: JsonPropertyName("token_type")] string TokenType
    );

    public class Twitch : ITwitch
    {
        private readonly HttpClient _client;
        private readonly AppConfig _cfg;
        private readonly TwitchAPI _twitch;
        private static string TwitchCfgKey = "Twitch";

        private async Task<AccessTokenResponse> GetAccessToken()
        {
            var url = $"https://id.twitch.tv/oauth2/token?" +
                $"client_id={_cfg.TwitchClientId}&" +
                $"client_secret={_cfg.TwitchSecretToken}&" +
                $"grant_type=client_credentials";
            var res = await _client.PostAsync(url, null);
            res.EnsureSuccessStatusCode();
            return await res.Content.ReadFromJsonAsync<AccessTokenResponse>() ?? throw new ArgumentNullException();
        }
        public Twitch(HttpClient client, AppConfig cfg, TwitchAPI twitch) 
        {
            _client = client;
            _cfg = cfg;
            _twitch = twitch; ;
        }
        static string ExtractUsernameFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            Uri uri = new Uri(url);
            if (uri.Host != "www.twitch.tv" && uri.Host != "twitch.tv")
                throw new ArgumentException("URL is not a valid Twitch URL", nameof(url));

            string path = uri.AbsolutePath;
            if (string.IsNullOrEmpty(path) || path == "/")
                throw new ArgumentException("URL does not contain a username", nameof(url));

            return path.Trim('/');
        }
        private string GetTwitchLinkPlayer(string channel)
        {
            return $"https://player.twitch.tv/?channel={channel}&parent={_cfg.Host}";
        }
        public async Task<UsersOnlineCheckResponse> CheckUsersInOnline(List<Streamer> streamerBatch)
        {
            var groupedSocials = streamerBatch
                .ToDictionary(s => s.Id, s => s.Socials);
            if (groupedSocials.Sum(s => s.Value.Count()) > 100)
                throw new ArgumentException(AppDictionary.ExceedLimit, nameof(streamerBatch));
            var channelsToCheck = groupedSocials
                .SelectMany(s => s.Value)
                .Select(s => ExtractUsernameFromUrl(s.Link))
                .ToList();
            string? cursor = null;
            var result = new Dictionary<long, List<TwitchBroadcastInformation>>();
            while (true)
            {
                var res = await _twitch.Helix.Streams.GetStreamsAsync(userLogins: channelsToCheck, first: 100, after:cursor);
                var channelsOnline = res.Streams.Select(t => t.UserLogin);
                var streamersId = streamerBatch.Select(s => s.Id);
                foreach (var streamerId in streamersId)
                {
                    var channelsForThisStreamer = groupedSocials[streamerId]
                        .Select(s => ExtractUsernameFromUrl(s.Link));
                   foreach(var channel in channelsOnline)
                    {
                        if (channelsForThisStreamer.Contains(channel))
                        {
                            if (!result.ContainsKey(streamerId))
                                result[streamerId] = new();
                            result[streamerId].Add(new() 
                            { 
                                ChannelName = channel, 
                                IsLive = true, 
                                Link = GetTwitchLinkPlayer(channel)
                            });
                        }
                    }
                }
                cursor = res.Pagination.Cursor;
                if (!res.Streams.Any())
                    break;
            }
            return new() { BroadcastInformation = result };
        }
    }
}