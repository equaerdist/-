using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services.UserRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<AppCtx> _factory;

        public UserRepository(IDbContextFactory<AppCtx> factory) 
        {
            _factory = factory;
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
    }
}
