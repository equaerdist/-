namespace backend_iGamingBot
{
    public class UserPayMethod
    {
        public string Platform { get; set; } = null!;
        public string? Data { get; set; }
        public long UserId { get; set; }
        public DefaultUser? User { get; set; }
    }
}
