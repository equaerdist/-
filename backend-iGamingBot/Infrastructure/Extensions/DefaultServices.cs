using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure
{
    public static class DefaultServices
    {
        public static IServiceCollection AddInftrastructureServices(this IServiceCollection services, AppConfig cfg)
        {
            services.AddHttpClient();
            services.AddDbContext<AppCtx>(opt => opt.UseNpgsql(cfg.SqlKey));
            services.AddSingleton(cfg);
            return services;
        }
    }
}
