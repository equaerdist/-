namespace backend_iGamingBot
{
    public class Config
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTime ExpirationTime { get; set; }
        public object Payload { get; set; } = null!;
    }
}
