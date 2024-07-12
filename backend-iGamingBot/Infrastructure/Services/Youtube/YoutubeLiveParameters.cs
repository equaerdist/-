namespace backend_iGamingBot.Infrastructure.Services
{
    public class YoutubeLiveParameters : ILiveParameter
    {
        public bool IsLive { get; set; }
        public string? Link { get; set; }
        public string Identifier { get; set; } = null!;
    }
}
