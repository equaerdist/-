namespace backend_iGamingBot.Dto
{
    public class AdminInviteResponse
    {
        public string Name { get; set; } = null!;
        public Guid Code { get; set; }
        public string Link { get; set; } = null!;
    }
}
