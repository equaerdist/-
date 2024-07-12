
using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly AppCtx _ctx;

        public ConfigRepository(AppCtx ctx) 
        { 
            _ctx = ctx;
        }
        public async Task<Config?> GetConfigByNameAsync(string name)
        => await _ctx.Configs.FirstOrDefaultAsync(c => c.Name.Equals(name));

        public async Task RemoveConfigByNameAsync(string name) 
            => await _ctx.Configs.Where(c => c.Name == name).ExecuteDeleteAsync();

        public async Task SetupConfigAsync(Config config)
        => await _ctx.Configs.AddAsync(config);
    }
}
