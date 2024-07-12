using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class StreamerRepository : IStreamerRepository
    {
        private readonly AppCtx _ctx;

        public StreamerRepository(AppCtx ctx) 
        {
            _ctx = ctx;
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
    }
}
