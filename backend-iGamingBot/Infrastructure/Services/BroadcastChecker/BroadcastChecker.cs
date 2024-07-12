namespace backend_iGamingBot.Infrastructure.Services
{
    public static class ExtensionsForBroadcastChecker
    {
        public static IEnumerable<Streamer> GetStreamersWithSocial(this IEnumerable<Streamer> streamers, string social)
            => streamers.Where(s => s.Socials.Where(l => l.Name.Equals(social)).Any());
        public static IEnumerable<Streamer> FilterSocialsWithName(this IEnumerable<Streamer> streamers, string name)
            => streamers.Select(t => new Streamer()
            {
                Id = t.Id,
                Name = t.Name,
                Socials = t.Socials.Where(l => l.Name == name).ToList(),
            });
    }
    public class BroadcastChecker : BackgroundService
    {
        private static TimeSpan CheckingDelayForYoutube => TimeSpan.FromSeconds(1);
        private static TimeSpan CheckingDelayForTwitch => TimeSpan.FromSeconds(2);
        private static TimeSpan CheckingDelay => TimeSpan.FromMinutes(1);
        private IServiceProvider _servSrc;
        private static string Youtube = GetSocialNameConstant(nameof(Youtube));
        private static string Twitch = GetSocialNameConstant(nameof(Twitch));
        private ILogger logger = null!;
        private IUnitOfWork unitOfWork = null!;
        private IStreamerRepository streamersSrc = null!;
        private ITwitch twitch = null!;
        private IYoutube youtube = null!;
        public BroadcastChecker(IServiceProvider servSrc) 
        { 
            _servSrc = servSrc;
        }
        private static string GetSocialNameConstant(string social)
        {
            return AppDictionary.ResolvedSocialNames
                       .Where(s => s.name.Equals(social, StringComparison.OrdinalIgnoreCase))
                       .First().name;
        }
        private async Task CheckYoutubeStreamersOnBroadcast(Streamer[] fulled, 
            Streamer[] onlyWithNeedSocials)
        {
            foreach (var streamer in onlyWithNeedSocials)
            {
                foreach(var social in streamer.Socials)
                {
                    var ytbPrm = social.Parameter as YoutubeLiveParameters;
                    if (ytbPrm is null)
                        throw new InvalidProgramException();
                    try
                    {
                        logger.LogDebug($"Делаю запрос на платформу ютуб проверки стрима для {streamer.Name}\n" +
                            $"Channel - [{social.Link}].");
                        ILiveParameter streamInfo = await youtube.UserIsStreaming(ytbPrm.Identifier);
                        social.Parameter.Link = streamInfo.Link;
                        social.Parameter.IsLive = streamInfo.IsLive;
                        await Task.Delay((int)CheckingDelayForYoutube.TotalSeconds);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Вознилка шоибка при проверке стриминга ютуб для {streamer.Name}\n" +
                            $"Channel [{social.Name}]\n" +
                            $"{ex.Message}");
                    }
                }
                var streamerFromDb = fulled.First(s => s.Id == streamer.Id);
                var result = streamerFromDb.Socials.Where(s => s.Name != Youtube).ToList();
                result.AddRange(streamer.Socials);
                streamerFromDb.Socials = result;
                await unitOfWork.SaveChangesAsync();
            }
        }
        private async Task CheckTwitchStreamersOnBroadcast(Streamer[] fulled,
            Streamer[] onlyWithNeedSocials)
        {
            int cursor = 0;
            while (true)
            {
                var firstBatchSize = 0;
                List<Streamer> streamersBatch = new List<Streamer>();
                var maxTwitchChannelsCount = 100;
                for (int i = cursor; i < onlyWithNeedSocials.Length; i++)
                {
                    {
                        Streamer streamer = onlyWithNeedSocials[i];
                        if (streamer.Socials.Count + firstBatchSize < maxTwitchChannelsCount)
                        {
                            streamersBatch.Add(streamer);
                            firstBatchSize++;
                        }
                        else
                        {
                            firstBatchSize = 0;
                            break;
                        }
                    }
                    try
                    {
                        logger.LogDebug($"Начинаю отправку батчей для твитча на проверку броадкаста...");
                        var result = await twitch.CheckUsersInOnline(streamersBatch);
                        foreach (var streamer in fulled)
                        {
                            if (result.BroadcastInformation.Keys.Contains(streamer.Id))
                            {
                                var finalSocials = streamer.Socials.Where(s => s.Name != Twitch).ToList();
                                finalSocials.AddRange(result.BroadcastInformation[streamer.Id]
                                    .Select(t => new Social()
                                    {
                                        Link = twitch.ConstructTwitchUrl(t.ChannelName),
                                        Name = Twitch,
                                        Parameter = t
                                    }));
                                streamer.Socials = finalSocials;
                                await unitOfWork.SaveChangesAsync();
                            }
                        }
                        await Task.Delay((int)CheckingDelayForTwitch.TotalSeconds);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Возникла ошибка при проверке броадкаста для батча твитча {cursor}\n" +
                            $"{ex.Message}");
                    }
                }
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(async () =>
        {
            using var scope = _servSrc.CreateScope();
            logger = scope.ServiceProvider.GetRequiredService<ILogger<BroadcastChecker>>();
            streamersSrc = scope.ServiceProvider.GetRequiredService<IStreamerRepository>();
            twitch = scope.ServiceProvider.GetRequiredService<ITwitch>();
            youtube = scope.ServiceProvider.GetRequiredService<IYoutube>();
            unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            int page = 1;
            int pageSize = 100;
            while (true)
            {
                while (true)
                {
                    var batch = await streamersSrc.GetStreamerBatchAsync(page, pageSize);
                    var streamersWithYoutube = batch.GetStreamersWithSocial(Youtube);
                    var streamersWithTwitch = batch.GetStreamersWithSocial(Twitch);
                    streamersWithTwitch = streamersWithTwitch.FilterSocialsWithName(Twitch);
                    streamersWithYoutube = streamersWithYoutube.FilterSocialsWithName(Youtube);
                    var currentTasks = new Task[2];
                    if (streamersWithTwitch != null)
                        currentTasks[0] = CheckTwitchStreamersOnBroadcast(batch, streamersWithTwitch.ToArray());
                    if (streamersWithYoutube != null)
                        currentTasks[1] = CheckYoutubeStreamersOnBroadcast(batch, streamersWithYoutube.ToArray());
                    await Task.WhenAll(currentTasks);
                    page++;
                    if (batch.Length < pageSize)
                        break;
                }
                page = 1;
                await Task.Delay((int)CheckingDelay.TotalSeconds);
            }
        });
    }
}
