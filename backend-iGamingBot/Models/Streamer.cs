
namespace backend_iGamingBot
{
    public class Streamer
    {
        public Streamer() 
        { 
            Socials = new List<Social>();
            Subscribers = new List<User>();
            CreatedRaffles = new List<Raffle>();
        }
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string TgId { get; set; } = null!;
        public List<Social> Socials { get; set; }
        public ICollection<User> Subscribers { get; set; } = null!;
        public ICollection<Subscriber> SubscribersRelation { get; set; } = null!;
        public ICollection<Raffle> CreatedRaffles { get; set; } = null!;
    }
}
