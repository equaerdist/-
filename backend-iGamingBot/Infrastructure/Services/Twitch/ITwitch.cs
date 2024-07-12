namespace backend_iGamingBot.Infrastructure.Services
{
    public interface ITwitch
    {
        public Task<UsersOnlineCheckResponse> CheckUsersInOnline(List<Streamer> streamerBatch);
        public  string ExtractUsernameFromUrl(string url);
        public  string ConstructTwitchUrl(string username);
    }
}
