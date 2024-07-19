namespace backend_iGamingBot.Models
{
    public class WinnerNote
    {
        public long WinnerId { get; set; }
        public DefaultUser? Winner { get; set; }
        public long RaffleId { get; set; }
        public Raffle? Raffle { get; set; }
        public int AmountOfWins { get; set; }
    }
}
