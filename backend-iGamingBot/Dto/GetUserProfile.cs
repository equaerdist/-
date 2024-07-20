namespace backend_iGamingBot.Dto
{
    public class GetUserProfile
    {
        public long Id { get; set; }
        public string TgId { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public ICollection<GetUserPayMethod> UserPayMethods { get; set; } = null!;
    }
}
