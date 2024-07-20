namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IRaffleService
    {
        public Task GenerateWinnersForRaffle(long raffleId, bool exceptRepeat, string? sourceId = null,
            int? amountOfWinners = null);
    }
}
