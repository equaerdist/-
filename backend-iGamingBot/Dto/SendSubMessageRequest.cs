namespace backend_iGamingBot.Dto
{
    public class SendSubMessageRequest
    {
        public string StreamerId { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string Id { get; set; } = null!;
    }
}
