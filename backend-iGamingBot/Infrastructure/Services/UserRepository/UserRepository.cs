using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<AppCtx> _factory;
        private readonly AppCtx _ctx;

        public UserRepository(IDbContextFactory<AppCtx> factory, AppCtx ctx) 
        {
            _factory = factory;
            _ctx = ctx;
        }
        public async Task<string> DefineRoleByTgIdAsync(string tgId)
        {
            using var streamerCtx = await _factory.CreateDbContextAsync();
            using var userCtx = await _factory.CreateDbContextAsync();
            var streamerTask = streamerCtx.Streamers.Select(s => s.TgId).FirstOrDefaultAsync(s => s == tgId);
            var userTask = userCtx.Users.Select(u => u.TgId).FirstOrDefaultAsync(u => u == tgId);
            await Task.WhenAll(streamerTask, userTask);
            if (streamerTask.Result != null)
                return AppDictionary.StreamerRole;
            if(userTask.Result != null)
                return AppDictionary.UserRole;
            throw new AppException(AppDictionary.UserNotExists);
        }

        public async Task<DefaultUser> GetUserByIdAsync(string tgId)
        {
            return await _ctx.AllUsers.FirstAsync(s => s.TgId == tgId);
        }

        public async Task<long> GetUserIdByTgIdAsync(string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.AllUsers
                .Where(c => c.TgId == tgId)
                .Select(s => s.Id)
                .FirstAsync();
        }
    }
}
