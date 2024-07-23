using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class TelegramPostRequest
    {
        public PostCreatorFile? Media { get; set; }
        public string StreamerId { get; set; } = null!;
        public long[] Viewers { get; set; } = null!;
        public string? Message { get; set; }
    }
}
