namespace backend_iGamingBot.Models
{
    public class StreamerInvite
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public Guid Code { get; set; }
    }
}
