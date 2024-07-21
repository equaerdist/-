namespace backend_iGamingBot.Dto
{
    public class GetSubParticipant
    {
        public long Id { get; set; }
        public string Status { get; set; } = null!;
        public DateTime EndTime { get; set; }
    }
}
