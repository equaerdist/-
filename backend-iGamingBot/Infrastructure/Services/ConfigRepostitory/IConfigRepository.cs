namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IConfigRepository
    {
        public Task<Config?> GetConfigByNameAsync(string name);
        public Task SetupConfigAsync(Config config);
    }
}
