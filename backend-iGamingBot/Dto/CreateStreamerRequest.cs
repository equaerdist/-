namespace backend_iGamingBot.Dto
{
    public class CreateStreamerRequest
    {
        public string Name { get; set; } = null!;
        public string TgId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public string? LastName { get; set; }
    }
}
