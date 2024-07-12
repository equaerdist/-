
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
        private IServiceProvider _servSrc;
        private static string Youtube = GetSocialNameConstant(nameof(Youtube));
        private static string Twitch = GetSocialNameConstant(nameof(Twitch));

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
        private static async Task CheckYoutubeStreamersOnBroadcast(Streamer[] fulled, 
            Streamer[] onlyWithNeedSocials, 
            IYoutube yuotube,
            IUnitOfWork uof)
        {
            foreach (var streamer in onlyWithNeedSocials)
            {
                foreach(var social in streamer.Socials)
                {
                    var ytbPrm = social.Parameter as YoutubeLiveParameters;
                    if (ytbPrm is null)
                        throw new InvalidProgramException();
                    ILiveParameter streamInfo = await yuotube.UserIsStreaming(ytbPrm.Identifier);
                    social.Parameter.Link = streamInfo.Link;
                    social.Parameter.IsLive = streamInfo.IsLive;
                    await Task.Delay((int)CheckingDelayForYoutube.TotalSeconds);
                }
                var streamerFromDb = fulled.First(s => s.Id == streamer.Id);
                var result = streamerFromDb.Socials.Where(s => s.Name != Youtube).ToList();
                result.AddRange(streamer.Socials);
                streamerFromDb.Socials = result;
                await uof.SaveChangesAsync();
            }
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(async () =>
        {
            using var scope = _servSrc.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<BroadcastChecker>>();
            var streamerSrc = scope.ServiceProvider.GetRequiredService<IStreamerRepository>();
            var twitch = scope.ServiceProvider.GetRequiredService<ITwitch>();
            var youtube = scope.ServiceProvider.GetRequiredService<IYoutube>();
            var uof = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            int page = 1;
            int pageSize = 100;
            while(true)
            {
                var batch = await streamerSrc.GetStreamerBatchAsync(page, pageSize);
                var streamersWithYoutube = batch.GetStreamersWithSocial(Youtube);
                var streamersWithTwitch = batch.GetStreamersWithSocial(Twitch);
                streamersWithTwitch = streamersWithTwitch.FilterSocialsWithName(Twitch);
                streamersWithYoutube = streamersWithYoutube.FilterSocialsWithName(Youtube);
            }
        });
    }
}
