using backend_iGamingBot.Infrastructure.Services;
using backend_iGamingBot.Infrastructure.Services.RaffleRepository;
using backend_iGamingBot.Infrastructure.Services.RafflesEnder;
using Telegram.Bot;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;

namespace backend_iGamingBot.Infrastructure
{
    public static class AppServices
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, AppConfig cfg)
        {
            #region telegram
            services.AddHttpClient("telegram_bot_client")
               .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
               {
                   TelegramBotClientOptions options = new(cfg.TgKey);
                   return new TelegramBotClient(options, httpClient);
               });

            services.AddScoped<UpdateHandler>();
            services.AddScoped<ReceiverService>();
            services.AddHostedService<PollingService>();
            #endregion
            services.AddSingleton<ITwitchAPI, TwitchAPI>(opt => new());
            services.AddScoped<IYoutube, Youtube>();
            services.AddScoped<ITwitch, Twitch>();
            services.AddScoped<IConfigRepository, ConfigRepository>();
            services.AddScoped<IStreamerRepository, StreamerRepository>();
            services.AddHostedService<BroadcastChecker>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStreamerService, StreamerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRaffleRepository, RaffleRepository>();
            services.AddScoped<IRaffleService, RaffleService>();
            services.AddTransient<ExceptionHandler>();
            services.AddSingleton<TelegramPostCreator>();
            services.AddScoped<ISubscriberRepository, SubscriberRepository>();
            services.AddScoped<ISubscriberService, SubscriberService>();
            services.AddScoped<IAuth, Auth>();
            services.AddSingleton<IHostedService, TelegramPostCreator>(
                       serviceProvider => serviceProvider.GetRequiredService<TelegramPostCreator>());
            services.AddHostedService<RafflesEnder>();
            return services;
        }
    }
}
