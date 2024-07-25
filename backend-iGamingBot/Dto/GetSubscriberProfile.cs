namespace backend_iGamingBot.Dto
{
    public class GetSubscriberProfile
    {
        public long Id { get; set; }
        public string TgId { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? LastName { get; set; }
        public DateTime SubscribeTime { get; set; }
        public string? Note { get; set; }
        public ICollection<GetUserPayMethod> UserPayMethods { get; set; } = null!;
        public SubscriberStat SubscriberStat { get; set; } = null!;
    }
}
