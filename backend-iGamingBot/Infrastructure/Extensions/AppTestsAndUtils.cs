﻿using AngleSharp.Html.Parser;
using AngleSharp.Io;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure.Configs;
using backend_iGamingBot.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace backend_iGamingBot.Infrastructure.Extensions
{
    public static class FakeRafflesFactory
    {
        public static int MaxValue => 1000;
        public static Raffle[] GetRaffles() =>
        Enumerable.Range(1, 500).Select(t => new Raffle()
        {
            AmountOfWinners = Random.Shared.Next(1, 5),
            RaffleConditions = [],
            Description = $"Smth description_ \n{Guid.NewGuid()}\n{Guid.NewGuid()}",
            EndTime = DateTime.UtcNow + TimeSpan.FromDays(MaxValue),
            ShowWinners = Random.Shared.Next(0, 2) == 1 ,
            ShouldNotifyUsers = Random.Shared.Next(0,2) == 1 ,
        }).ToArray();

    }
    public static class FakeSubscribersFactory
    {
        private static int MaxValue => 1000;
        public static Subscriber[] GetSubscribers() =>
        Enumerable.Range(1, 2000).Select(t => new Subscriber()
        {
            SubscribeTime = DateTime.UtcNow - TimeSpan.FromDays(Random.Shared.Next(MaxValue)),
            User = new User()
            {
                FirstName = $"Peter Parker_{Random.Shared.Next(MaxValue)}_{Guid.NewGuid().ToString()}",
                TgId = Random.Shared.Next(MaxValue).ToString() + Guid.NewGuid().ToString(),
            }
        }).ToArray();
    }
    public static class FakeStreamersFactory
    {
        private static int MaxValue { get; } = 1000;
        public async static Task<Streamer[]> GetStreamers(IYoutube yt) =>
            [
                new()
                {
                    TgId = Random.Shared.Next(MaxValue).ToString(),
                    Name = "recrent",
                    FirstName = "recr",
                    SubscribersRelation = FakeSubscribersFactory.GetSubscribers(),
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
                    TgId = Random.Shared.Next(MaxValue).ToString(),
                    Name = "nix",
                    FirstName = "nix",
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
                    FirstName = "ct0m",
                     TgId = Random.Shared.Next(MaxValue).ToString(),
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
                     TgId = Random.Shared.Next(MaxValue).ToString(),
                    Name = "kasanofff",
                    FirstName = "kagh",
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
                    TgId = Random.Shared.Next(MaxValue).ToString(),
                    Name = "VIRTUALNEWYORK",
                    FirstName = "232323",
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
                    TgId = Random.Shared.Next(MaxValue).ToString(),
                    Name = "Kolezev",
                    FirstName = "4543533",
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
                    TgId = Random.Shared.Next(MaxValue).ToString(),
                    SubscribersRelation = FakeSubscribersFactory.GetSubscribers(),
                    CreatedRaffles = FakeRafflesFactory.GetRaffles(),
                    Name = "LofiGirl",
                    FirstName = "gfgd",
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
            ];
    }
    public static class FakeUserFactory
    {
        private static string TestTgId = "5";
        public static User GetUser()
        {
            return new User()
            {
                FirstName = "Peter",
                TgId = TestTgId,

            };
        }
    }
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
        public static async Task<WebApplication> CreateFakeStreamersWithSubscribers(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var yt = scope.ServiceProvider.GetRequiredService<IYoutube>();
            var ctx = scope.ServiceProvider.GetRequiredService<AppCtx>();
            var streamers = await FakeStreamersFactory.GetStreamers(yt);
            await ctx.Streamers.AddRangeAsync(streamers);
            await ctx.SaveChangesAsync();
            return app;
        }
        public static async Task<WebApplication> CreateInfoForStreamersPage(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<AppCtx>();
            var testUser = FakeUserFactory.GetUser();
            var yt = scope.ServiceProvider.GetRequiredService<IYoutube>();
            var streamers = await FakeStreamersFactory.GetStreamers(yt);
            var randomStreamer = streamers.First();
            var finalSubscribers = randomStreamer.SubscribersRelation.ToList();
            finalSubscribers.Add(new() { SubscribeTime = DateTime.UtcNow, User = testUser });
            randomStreamer.SubscribersRelation = finalSubscribers;
            await ctx.Streamers.AddRangeAsync(streamers);
            await ctx.SaveChangesAsync();
            return app;
        }
        public static async  Task<WebApplication> TestFunction(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userSrc = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var streamer = await userSrc.GetUserByIdAsync("272");
            var user = await userSrc.GetUserByIdAsync("5");
            var isStreamer = streamer is Streamer;
            var isUser = user is User;
            throw new InvalidCastException("Тестовая функция");
        }
        public static async Task<WebApplication> CreateTestRaffleForUser(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userSrc = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var streamer = await userSrc.GetUserByIdAsync("272");
            var ctx = scope.ServiceProvider.GetRequiredService<AppCtx>();
            var repeatedPayMethods = AppDictionary.DefaultPayMethods;
            repeatedPayMethods.First(c => c.Platform == AppDictionary.TetherERC20)
                .Data = "0x742d35cc6634c0532925a3b844bc454e4438f44e";
            var users = Enumerable.Range(0, 10).Select(i => new DefaultUser()
            {
                FirstName = $"Peter {i}",
                TgId = (i * 21 + 18).ToString(),
                UserPayMethods = AppDictionary.DefaultPayMethods.ToList(),
                StreamersRelation = [new() 
                {
                    Note = $"Peter {i}",
                    Streamer = streamer as Streamer, 
                    SubscribeTime = DateTime.UtcNow
                }]
            }).ToList();
            foreach(var user in users.Take(5))
            {
                user.UserPayMethods =
                    [new()
                    {
                        Platform = AppDictionary.TetherERC20,
                        Data = "0x742d35cc6634c0532925a3b844bc454e4438f44e"
                    }];
            }
            var raffle = new Raffle()
            {
                AmountOfWinners = 7,
                CreatorId = streamer.Id,
                Description = "Какой-то розыгрыш",
                EndTime = DateTime.UtcNow + TimeSpan.FromSeconds(30),
                Participants = users.ToList(),
            };
            await ctx.Raffles.AddAsync(raffle);
            await ctx.SaveChangesAsync();
            
            return app;
        }
        public static async Task<WebApplication> CreateTestUser(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userSrc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var user = await userSrc.RegisterUser(new() { FirstName = "Peter", TgId = "567765" });
            return app;
        }
        public static async Task<WebApplication> CreateTestYoutubeChannel(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userSrc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var user = await userSrc.RegisterStreamer(new() 
            { 
                FirstName = "Equaer?", 
                TgId = "99999" ,
                Name = "Equaerdist??"
            });
            return app;
        }
        public static async Task<WebApplication> StreamersTest(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userSrc = scope.ServiceProvider.GetRequiredService<IUserService>();
            var user = await userSrc.RegisterStreamer(new()
            {
                FirstName = "Equaersd",
                TgId = "999991",
                Name = "Equaerdist??f"
            });
            var userTwo = await userSrc.RegisterStreamer(new()
            {
                FirstName = "Equaerdfds",
                TgId = "999991",
                Name = "Equaerdist??s"
            });
            return app;
        }
        public static async Task<WebApplication> PostTestWithFile(this WebApplication app)
        {
            long subscriber = 1341625052;
            long streamer = 1341625052;
            using var scope = app.Services.CreateScope();
            var excel = scope.ServiceProvider.GetRequiredService<IExcelReporter>();
            User[] users = [new User() { FirstName = "dfddd" }];
            var report = await excel.GenerateExcel(users.ToList());
            var tgPublisher = scope.ServiceProvider.GetRequiredService<TelegramPostCreator>();
            var post = new TelegramPostRequest() 
            { 
                Media = report, 
                Message = "Тетсовое сообщение ведь да",
                StreamerId = streamer.ToString(),
                Viewers = [subscriber]
            };
            tgPublisher.AddPostToLine(post);
            return app;
        }
    }
}
