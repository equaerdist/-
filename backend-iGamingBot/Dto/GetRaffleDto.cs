namespace backend_iGamingBot.Dto
{
    public class GetRaffleDto
    {
        public long Id { get; set; }
        public int AmountOfWinners { get; set; }
        public List<object> RaffleConditions { get; set; } = new();
        public string Description { get; set; } = null!;
        public DateTime EndTime { get; set; }
        public int AmountOfParticipants { get; set; }
        public bool ShowWinners { get; set; }
        public bool IsParticipant {  get; set; }
        public bool IsCreator { get; set; }
    }
}
