namespace backend_iGamingBot.Infrastructure.Services
{
    public class StreamInformation : ILiveParameter
    {
        public bool IsLive { get; set; }
        public string? Link { get; set; }
    }
}
