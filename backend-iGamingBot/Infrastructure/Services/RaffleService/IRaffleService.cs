namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IRaffleService
    {
        public Task GenerateWinnersForRaffle(long raffleId, bool shouldBeRepeat, int? amountOfWinners = null);
    }
}
