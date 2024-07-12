using backend_iGamingBot.Infrastructure.Services;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;

namespace backend_iGamingBot.Infrastructure
{
    public static class AppServices
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, AppConfig cfg)
        {

            services.AddSingleton<ITwitchAPI, TwitchAPI>(opt => new());
            services.AddScoped<IYoutube, Youtube>();
            services.AddScoped<ITwitch, Twitch>();
            services.AddScoped<IConfigRepository, ConfigRepository>();
            services.AddScoped<IStreamerRepository, StreamerRepository>();
            services.AddHostedService<BroadcastChecker>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
