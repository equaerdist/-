
using backend_iGamingBot.Infrastructure.Configs;

namespace backend_iGamingBot.Infrastructure.Services.RaffleRepository
{
    public class RaffleRepository : IRaffleRepository
    {
        private readonly AppCtx _ctx;

        public RaffleRepository(AppCtx ctx) 
        {
            _ctx = ctx;
        }
        public async Task CreateRaffleAsync(Raffle raffle) => await _ctx.Raffles.AddAsync(raffle);
    }
}
