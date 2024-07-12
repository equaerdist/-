namespace backend_iGamingBot.Infrastructure.Services.Twitch
{
    public class TwitchBroadcastInformation : ILiveParameter
    {
        public string ChannelName { get; set; } = null!;
        public bool IsLive { get; set; }
        public string? Link { get; set; }
    }
}
