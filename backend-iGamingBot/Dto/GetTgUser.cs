namespace backend_iGamingBot.Dto
{
    public class GetTgUser
    {
        public long Id { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? ImageUrl { get; set; }
        public string TgId { get; set; } = null!;
    }
}
