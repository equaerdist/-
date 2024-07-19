namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IRaffleRepository
    {
        public  Task CreateRaffleAsync(Raffle raffle);
    }
}
