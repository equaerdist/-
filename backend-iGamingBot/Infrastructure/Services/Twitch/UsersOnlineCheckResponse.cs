namespace backend_iGamingBot.Infrastructure.Services.Twitch
{
    public class UsersOnlineCheckResponse
    {
        public Dictionary<long, List<TwitchBroadcastInformation>> BroadcastInformation { get; set; } = null!;
    }
}
