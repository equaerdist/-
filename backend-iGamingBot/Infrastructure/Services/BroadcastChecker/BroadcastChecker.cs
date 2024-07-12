﻿using backend_iGamingBot.Models.Essentials;
using System.Linq;
using System.Text.Json;

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
      
        private ILogger logger = null!;
        private IUnitOfWork unitOfWork = null!;
        private IStreamerRepository streamersSrc = null!;
        private ITwitch twitch = null!;
        private IYoutube youtube = null!;
        public BroadcastChecker(IServiceProvider servSrc) 
        { 
            _servSrc = servSrc;
        }
       
        private async Task CheckYoutubeStreamersOnBroadcast(Streamer[] fulled, 
            Streamer[] onlyWithNeedSocials)
        {
            foreach (var streamer in onlyWithNeedSocials)
            {
                foreach(var social in streamer.Socials)
                {
                    var ytbPrm = social.Parameter;
                    if (ytbPrm is null)
                        throw new InvalidProgramException();
                    try
                    {
                        logger.LogDebug($"Делаю запрос на платформу ютуб проверки стрима для {streamer.Name}\n" +
                            $"Channel - [{social.Link}].");
                        DefaultLiveParameter streamInfo = await youtube.UserIsStreaming(ytbPrm.Identifier!);
                        social.Parameter = new ()
                        {
                            IsLive = streamInfo.IsLive,
                            Link = streamInfo.Link,
                            Identifier = ytbPrm.Identifier
                        };
                        await Task.Delay((int)CheckingDelayForYoutube.TotalSeconds * 1000);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Вознилка шоибка при проверке стриминга ютуб для {streamer.Name}\n" +
                            $"Channel [{social.Name}]\n" +
                            $"{ex.Message}");
                    }
                }
                var streamerFromDb = fulled.First(s => s.Id == streamer.Id);
                var result = streamerFromDb.Socials.Where(s => s.Name != AppDictionary.Youtube).ToList();
                result.AddRange(streamer.Socials);
                streamerFromDb.Socials.Clear();
               
                await unitOfWork.SaveChangesAsync();
                streamerFromDb.Socials.AddRange(result);
                if (streamerFromDb.Name == "LofiGirl")
                    Console.WriteLine();
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
                    foreach (var fulledStreamer in fulled)
                    {
                        if (result.BroadcastInformation.Keys.Contains(fulledStreamer.Id))
                        {

                            var twitchResult = result.BroadcastInformation[fulledStreamer.Id]
                            .Select(t => new Social()
                            {
                                Link = twitch.ConstructTwitchUrl(t.ChannelName),
                                Name = AppDictionary.Twitch,
                                Parameter = t
                            });
                            var twitchResultIds = twitchResult.Select(s => s.Link);
                            var notOnline = fulledStreamer.Socials
                                .Where(s => s.Name == AppDictionary.Twitch)
                                .Where(s => !twitchResultIds.Contains(s.Link))
                                .Select(s => new Social()
                                {
                                    Link = s.Link,
                                    Name = AppDictionary.Twitch,
                                    Parameter = new DefaultLiveParameter() { IsLive = false }
                                });
                            var finalSocials = fulledStreamer.Socials
                                .Where(s => s.Name != AppDictionary.Twitch)
                                .ToList();
                            finalSocials.AddRange(twitchResult);
                            finalSocials.AddRange(notOnline);
                            fulledStreamer.Socials = finalSocials;
                            await unitOfWork.SaveChangesAsync();
                        }
                    }
                    if (streamersBatch.SelectMany(s => s.Socials).Count() < maxTwitchChannelsCount)
                        break;
                    await Task.Delay((int)CheckingDelayForTwitch.TotalSeconds * 1000);
                }
                catch (Exception ex)
                {
                    logger.LogError($"Возникла ошибка при проверке броадкаста для батча твитча {cursor}\n" +
                        $"{ex.Message}");
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
                    if (batch.Length != 0)
                    {
                        var streamersWithYoutube = batch.GetStreamersWithSocial(AppDictionary.Youtube);
                        var streamersWithTwitch = batch.GetStreamersWithSocial(AppDictionary.Twitch);
                        streamersWithTwitch = streamersWithTwitch.FilterSocialsWithName(AppDictionary.Twitch);
                        streamersWithYoutube = streamersWithYoutube.FilterSocialsWithName(AppDictionary.Youtube);
                        await CheckTwitchStreamersOnBroadcast(batch, streamersWithTwitch.ToArray());
                        //await CheckYoutubeStreamersOnBroadcast(batch, streamersWithYoutube.ToArray());
                        page++;
                    }
                    if (batch.Length < pageSize)
                        break;
                }
                page = 1;
                await Task.Delay((int)CheckingDelay.TotalSeconds * 1000);
            }
        });
    }
}
