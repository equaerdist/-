
namespace backend_iGamingBot
{
    public class User
    {
        public User() 
        {
            PayMethods = new();
            Streamers = new List<Streamer>();
            ParticipantRaffles = new List<Raffle>();
            WinnerRaffles = new List<Raffle>();
        }
        public long Id { get; set; }
        public string TgId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public List<UserPayMethod> PayMethods { get; set; }
        public ICollection<Streamer> Streamers { get; set; } = null!;
        public ICollection<Subscriber> StreamersRelation { get; set; } = null!;
        public ICollection<Raffle> ParticipantRaffles { get;set; } = null!;
        public ICollection<Raffle> WinnerRaffles { get; set; } = null!;
    }
}
