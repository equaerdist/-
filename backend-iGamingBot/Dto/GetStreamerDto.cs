namespace backend_iGamingBot.Dto
{
    public class GetStreamerDto
    {
        public GetStreamerDto()
        {
            Socials = new();
        }
        public long Id { get; set; }
        public int AmountOfSubscribers { get; set; }
        public string Name { get; set; } = null!;
        public string TgId { get; set; } = null!;
        public bool IsLive { get; set; }
        public bool IsSubscribed { get; set; }
        public List<Social> Socials { get; set; }
        public string? ImageUrl { get; set; }
    }
}
