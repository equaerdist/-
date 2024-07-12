namespace backend_iGamingBot.Infrastructure.Services.Twitch
{
    public interface ITwitch
    {
        public Task<UsersOnlineCheckResponse> CheckUsersInOnline(List<Streamer> streamerBatch);
    }
}
