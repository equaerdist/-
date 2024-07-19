using AutoMapper;
using AutoMapper.QueryableExtensions;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services.RaffleRepository
{
    public class RaffleRepository : IRaffleRepository
    {
        private readonly AppCtx _ctx;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<AppCtx> _factory;

        public RaffleRepository(AppCtx ctx, IDbContextFactory<AppCtx> factory, IMapper mapper) 
        {
            _ctx = ctx;
            _mapper = mapper;
            _factory = factory;
        }
        public async Task CreateRaffleAsync(Raffle raffle) => await _ctx.Raffles.AddAsync(raffle);

        public async Task<GetRaffleDto> GetRaffleByIdAsync(long id)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var raffles = ctx.Raffles.Where(r => r.Id.Equals(id));
            return await raffles.Select(r => new GetRaffleDto()
            {
                AmountOfParticipants = r.Participants.Count,
                AmountOfWinners = r.AmountOfWinners,
                Description = r.Description,
                EndTime = r.EndTime,
                Id = r.Id,
            }).FirstAsync();
        }

        public async Task<GetSubscriberDto[]> GetRaffleWinners(long raffleId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.AllUsers
                .Where(u => u.WinnerRaffles.Select(r => r.Id).Contains(raffleId))
                .ProjectTo<GetSubscriberDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return result;
        }

        public async Task<Raffle> GetTrackingRaffleByIdAsync(long id)
        {
            return await _ctx.Raffles.Where(r => r.Id == id).FirstAsync();
        }
    }
}
