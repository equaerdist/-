using AutoMapper;
using AutoMapper.QueryableExtensions;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class StreamerRepository : IStreamerRepository
    {
        private readonly AppCtx _ctx;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<AppCtx> _factory;

        public StreamerRepository(AppCtx ctx, 
            IDbContextFactory<AppCtx> factory,
            IMapper mapper) 
        {
            _ctx = ctx;
            _mapper = mapper;
            _factory = factory;
        }

        public async Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.Raffles
                .Where(r => r.Creator!.TgId.Equals(tgId))
                .OrderBy(r => r.EndTime)
                .Skip((page- 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetRaffleDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return result;
        }

        public async Task<Streamer[]> GetStreamerBatchAsync(int page, int pageSize)
        {
            var batch = await _ctx.Streamers
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();
            return batch;
        }

        public async Task<GetStreamerDto> GetStreamerByTgIdAsync(string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var streamer = await ctx.Streamers
                .Where(s => s.TgId.Equals(tgId))
                .ProjectTo<GetStreamerDto>(_mapper.ConfigurationProvider)
                .FirstAsync();
            return streamer;
        }

        public async Task<GetSubscriberDto[]> GetSubscribersAsync(int page, int pageSize, string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.Subscribers
                .Where(s =>  s.User!.TgId.Equals(tgId))
                .OrderByDescending(s => s.SubscribeTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetSubscriberDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return result;
        }
    }
}
