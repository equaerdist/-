namespace backend_iGamingBot
{
    public class DefaultUser
    {
        public DefaultUser()
        {
            ParticipantRaffles = new List<Raffle>();
            WinnerRaffles = new List<Raffle>();
            Streamers =  new List<Streamer>();
            StreamersRelation = new List<Subscriber>();
            Negotiable = new List<Streamer>();
        }
        public long Id { get; set; }
        public string TgId { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public ICollection<Raffle> ParticipantRaffles { get; set; } = null!;
        public ICollection<Raffle> WinnerRaffles { get; set; } = null!;
        public ICollection<Streamer> Streamers { get; set; } = null!;
        public ICollection<Subscriber> StreamersRelation { get;set; } = null!;
        public ICollection<Streamer> Negotiable { get; set; } = null!;
    }
}
