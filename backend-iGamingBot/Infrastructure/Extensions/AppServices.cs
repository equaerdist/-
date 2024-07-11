using backend_iGamingBot.Infrastructure.Services;
using TwitchLib.Api;
using TwitchLib.Api.Interfaces;

namespace backend_iGamingBot.Infrastructure
{
    public static class AppServices
    {
        public static IServiceCollection AddAppServices(this IServiceCollection services, AppConfig cfg)
        {
           
            //services.AddSingleton<ITwitchAPI, TwitchAPI>(opt => API);
            services.AddScoped<IYoutube, Youtube>();
            return services;
        }
    }
}
