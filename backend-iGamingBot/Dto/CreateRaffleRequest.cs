using Microsoft.AspNetCore.Mvc;

namespace backend_iGamingBot.Dto
{
    public class CreateRaffleRequest
    {
        public int AmountOfWinners { get; set; }
        public bool ShowWinners { get; set; }
        public List<string> RaffleConditions { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime EndTime { get; set; }
        public bool ShouldNotifyUsers { get; set; }
    }
}
