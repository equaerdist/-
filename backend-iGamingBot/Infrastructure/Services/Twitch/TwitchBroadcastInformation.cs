using backend_iGamingBot.Models.Essentials;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class TwitchBroadcastInformation : DefaultLiveParameter
    {
        public string ChannelName { get; set; } = null!;
    
    }
}
