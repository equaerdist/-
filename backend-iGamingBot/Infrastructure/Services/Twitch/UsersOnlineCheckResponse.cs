namespace backend_iGamingBot.Infrastructure.Services
{
    public class UsersOnlineCheckResponse
    {
        public Dictionary<long, List<TwitchBroadcastInformation>> BroadcastInformation { get; set; } = null!;
    }
}
