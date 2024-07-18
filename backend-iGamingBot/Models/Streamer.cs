
namespace backend_iGamingBot
{
    public class Streamer : DefaultUser
    {
        public Streamer() 
        { 
            Socials = new List<Social>();
            Subscribers = new List<DefaultUser>();
            SubscribersRelation = new List<Subscriber>();
            CreatedRaffles = new List<Raffle>();
            Admins = new List<DefaultUser>();
        }
      
        public string Name { get; set; } = null!;
        public List<Social> Socials { get; set; }
        public ICollection<DefaultUser> Subscribers { get; set; } = null!;
        public ICollection<Subscriber> SubscribersRelation { get; set; } = null!;
        public ICollection<Raffle> CreatedRaffles { get; set; } = null!;
        public ICollection<DefaultUser> Admins { get; set; } = null!;
    }
}
