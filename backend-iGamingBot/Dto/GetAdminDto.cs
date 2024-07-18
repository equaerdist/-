namespace backend_iGamingBot.Dto
{
    public class GetAdminDto
    {
        public long Id { get; set; }
        public string TgId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
    }
}
