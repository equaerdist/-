

namespace backend_iGamingBot
{
    public class Raffle
    {
        public long Id { get;set; }
        public Raffle() 
        {
            RaffleConditions = new();
            Participants = new List<DefaultUser>();
            Winners = new List<DefaultUser>();
        }
        public int AmountOfWinners { get; set; }
        public bool ShowWinners { get; set; }
        public List<string> RaffleConditions { get; set; }
        public string Description { get; set; } = null!;
        public DateTime EndTime { get; set; }
        public bool ShouldNotifyUsers { get; set; }
        public long CreatorId { get;set; }
        public Streamer? Creator { get; set; }
        public ICollection<DefaultUser> Participants { get; set; } = null!;
        public ICollection<DefaultUser> Winners { get; set; } = null!;
    }
}
