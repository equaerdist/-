using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IRaffleRepository
    {
        public Task CreateRaffleAsync(Raffle raffle);
        public Task<GetRaffleDto> GetRaffleByIdAsync(long id);
        public Task<Raffle> GetTrackingRaffleByIdAsync(long id);
        public Task<GetSubscriberDto[]> GetRaffleWinners(long raffleId);
    }
}
