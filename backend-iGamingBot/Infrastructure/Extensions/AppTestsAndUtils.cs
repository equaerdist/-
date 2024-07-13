using backend_iGamingBot.Infrastructure.Configs;
using backend_iGamingBot.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Extensions
{
    public static class AppTestsAndUtils
    {
        public static async Task<WebApplication> TestOnYoutubeStreaming(this WebApplication app)
        {
            var youtubeNames = new string[] { "@LofiGirl", "@recrent", "@abc7NY", "@IlyaBodrovKrukowski", "@zeedthx" };
            using var scope = app.Services.CreateScope();
            var youtube = scope.ServiceProvider.GetRequiredService<IYoutube>();
            var result = new List<(string, StreamInformation)>();
            foreach (var name in youtubeNames)
            {
                var id = await youtube.GetUserIdentifierByLinkAsync($"https://www.youtube.com/{name}");
                var streamResult = await youtube.UserIsStreaming(id);
                result.Add((name, streamResult));
            }
            throw new InvalidOperationException("Программа была запущена с тестами");
        }
        public static async Task<WebApplication> CheckMigrationsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppCtx>();
            var migrations = await ctx.Database.GetPendingMigrationsAsync();
            if (migrations.Any())
                await ctx.Database.MigrateAsync();
            return app;
        }
        public static async Task<WebApplication> CreateFakeStreamers(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var yt = scope.ServiceProvider.GetRequiredService<IYoutube>();
            var ctx = scope.ServiceProvider.GetRequiredService<AppCtx>();
            List<Streamer> streamers = new()
            {
                new()
                {
                    Name = "recrent",
                    Socials = new()
                    {
                        new()
                        {
                            Name = AppDictionary.Twitch,
                            Link = "www.twitch.tv/recrent",
                            Parameter = new DefaultLiveParameter() { }
                        },
                        new()
                        {
                            Name = AppDictionary.Youtube,
                            Link = "www.youtube.com/@recrent",
                            Parameter = new ()
                            {
                                Identifier = await yt.GetUserIdentifierByLinkAsync("www.youtube.com/@recrent")
                            }
                        }
                    }
                },
                new()
                {
                    Name = "nix",
                    Socials = new()
                    {
                        new()
                        {
                            Name = AppDictionary.Twitch,
                            Link = "www.twitch.tv/nix",
                            Parameter = new DefaultLiveParameter() { }
                        }
                    }
                },
                new()
                {
                    Name = "ct0m",
                    Socials = new()
                    {
                        new()
                        {
                            Name = AppDictionary.Twitch,
                            Link = "www.twitch.tv/ct0m",
                            Parameter = new DefaultLiveParameter() { }
                        }
                    }
                },
                new()
                {
                    Name = "kasanofff",
                    Socials = new()
                    {
                        new()
                        {
                            Name = AppDictionary.Twitch,
                            Link = "www.twitch.tv/kasanofff",
                            Parameter = new DefaultLiveParameter() { }
                        }
                    }
                },
                new()
                {
                    Name = "VIRTUALNEWYORK",
                    Socials = new()
                    {
                        new()
                        {
                            Name = AppDictionary.Youtube,
                            Link = "www.youtube.com/@VIRTUALNEWYORK",
                            Parameter = new()
                            {
                                Identifier = await yt.GetUserIdentifierByLinkAsync("www.youtube.com/@VIRTUALNEWYORK")
                            }
                        }
                    }
                },
                new()
                {
                    Name = "Kolezev",
                    Socials = new()
                    {
                        new()
                        {
                            Name = AppDictionary.Youtube,
                            Link = "www.youtube.com/@Kolezev",
                            Parameter = new ()
                            {
                                Identifier = await yt.GetUserIdentifierByLinkAsync("www.youtube.com/@Kolezev")
                            }
                        }
                    }
                },
                new()
                {
                    Name = "LofiGirl",
                    Socials = new()
                    {
                        new()
                        {
                            Name = AppDictionary.Youtube,
                            Link = "www.youtube.com/@LofiGirl",
                            Parameter = new()
                            {
                                Identifier = await yt.GetUserIdentifierByLinkAsync("www.youtube.com/@LofiGirl")
                            }
                        }
                    }
                }
            };
            await ctx.Streamers.AddRangeAsync(streamers);
            await ctx.SaveChangesAsync();
            return app;
        }
    }
}
