namespace backend_iGamingBot.Dto
{
    public class GetSubscriberDto
    {
        public int Id { get; set; }
        public string TgId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public DateTime SubscribeTime { get; set; }
    }
}
