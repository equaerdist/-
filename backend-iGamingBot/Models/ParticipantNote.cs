namespace backend_iGamingBot.Models
{
    public class ParticipantNote
    {
        public long Id { get; set; }
        public long ParticipantId { get; set; }
        public DefaultUser? Participant {  get; set; }
        public long RaffleId { get; set; }
        public Raffle? Raffle { get; set; }
        public bool HaveAbused { get; set; } 
    }
}
