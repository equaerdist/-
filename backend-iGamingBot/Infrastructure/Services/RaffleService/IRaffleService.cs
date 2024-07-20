namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IRaffleService
    {
        public Task GenerateWinnersForRaffle(long raffleId, bool exceptRepeat, int? amountOfWinners = null);
    }
}
