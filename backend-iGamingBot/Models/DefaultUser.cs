using backend_iGamingBot.Models;

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
            ParticipantNotes = new List<ParticipantNote>();
            WinnerNotes = new List<WinnerNote>();
            UserPayMethods = new List<UserPayMethod>();
        }
        public long Id { get; set; }
        public string TgId { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string? Username { get; set; }
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; } = null!;
        public ICollection<Raffle> ParticipantRaffles { get; set; } = null!;
        public ICollection<ParticipantNote> ParticipantNotes { get; set; } = null!;
        public ICollection<Raffle> WinnerRaffles { get; set; } = null!;
        public ICollection<WinnerNote> WinnerNotes { get; set; } = null!;
        public ICollection<Streamer> Streamers { get; set; } = null!;
        public ICollection<Subscriber> StreamersRelation { get;set; } = null!;
        public ICollection<Streamer> Negotiable { get; set; } = null!;
        public ICollection<UserPayMethod> UserPayMethods { get; set; } = null!;
    }
}
