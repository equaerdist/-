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

        public SubscriberRepository(IDbContextFactory<AppCtx> factory, IMapper mapper) 
        {
            _factory = factory;
            _mapper = mapper;
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
    }
}
