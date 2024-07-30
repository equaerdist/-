namespace backend_iGamingBot.Models
{
    public interface IInvite
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Guid Code { get; set; }
    }
}
