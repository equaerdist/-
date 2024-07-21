using AutoMapper;
using AutoMapper.QueryableExtensions;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class SubscriberRepository : ISubscriberRepository
    {
        private readonly IDbContextFactory<AppCtx> _factory;
        private readonly IMapper _mapper;
        private readonly AppCtx _ctx;

        public SubscriberRepository(IDbContextFactory<AppCtx> factory, 
            IMapper mapper, AppCtx ctx) 
        {
            _factory = factory;
            _mapper = mapper;
            _ctx = ctx;
        }

        public async Task<GetSubParticipant[]> GetSubParticipants(string id, string streamerId, 
            int page, int pageSize)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.Participants
                .Where(s => s.Participant!.TgId == id
                    && s.Raffle!.Creator!.TgId == streamerId && s.Raffle.WinnersDefined)
                .OrderByDescending(s => s.Raffle!.EndTime)
                .Skip((page -1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetSubParticipant>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return result;
        }

        public async Task<GetSubscriberProfile> GetSubProfileByTgId(string id, string streamerId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.Subscribers
                .Where(s => s.Streamer!.TgId == streamerId && s.User!.TgId == id)
                .ProjectTo<GetSubscriberProfile>(_mapper.ConfigurationProvider)
                .FirstAsync();
            return result;
        }

        public async Task<Subscriber> GetSubscriberByTgId(string tgId, string streamerId)
        {
            return await _ctx.Subscribers
                .Where(s => s.Streamer!.TgId == streamerId && s.User!.TgId == tgId)
                .FirstAsync();
        }
    }
}
