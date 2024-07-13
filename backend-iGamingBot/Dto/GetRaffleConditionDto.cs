namespace backend_iGamingBot.Dto
{
    public class GetRaffleConditionDto
    {
        public bool IsDone { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
    }
}
