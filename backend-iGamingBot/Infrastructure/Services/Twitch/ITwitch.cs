namespace backend_iGamingBot.Infrastructure.Services.Twitch
{
    public interface ITwitch
    {
        public Task CheckUsersInOnline();
        public Task<Config?> GetConfig();
        public Task<Config> SetupConfig();
    }
}
