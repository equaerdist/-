using backend_iGamingBot.Automapper;
using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend_iGamingBot.Infrastructure
{
    public static class DefaultServices
    {

        public static IServiceCollection AddInftrastructureServices(this IServiceCollection services, AppConfig cfg)
        {
            services.AddHttpClient();
            services.AddAuthorization();
            services.AddAutoMapper(typeof(AppProfile));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = cfg.SymmetricSecurityKey,
                    ValidateIssuerSigningKey = true,
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        string accessToken = context.Request.Cookies["auth"] ?? string.Empty;
                        context.Token = accessToken;
                        return Task.CompletedTask;
                    }
                };
            });
            services.AddDbContext<AppCtx>(opt => opt.UseNpgsql(cfg.SqlKey), optionsLifetime:ServiceLifetime.Transient);
            services.AddDbContextFactory<AppCtx>();
            services.AddSingleton(cfg);
            return services;
        }
    }
}
