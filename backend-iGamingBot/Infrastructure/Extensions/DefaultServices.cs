namespace backend_iGamingBot.Infrastructure
{
    public static class DefaultServices
    {
        public static IServiceCollection AddInftrastructureServices(this IServiceCollection services)
        {
            services.AddHttpClient();
            return services;
        }
    }
}
