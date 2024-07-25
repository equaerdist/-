using AutoMapper;
using AutoMapper.QueryableExtensions;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<AppCtx> _factory;
        private readonly AppCtx _ctx;
        private readonly IMapper _mapper;

        public UserRepository(IDbContextFactory<AppCtx> factory, 
            AppCtx ctx,
            IMapper mapper) 
        {
            _factory = factory;
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task AddUserAsync(DefaultUser user) => await _ctx.AllUsers.AddAsync(user);

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

        public async Task<string> GetImageUrl(string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return UserResolver.ExtractFilePath(await ctx.AllUsers.Where(s => s.TgId == tgId)
                .Select(s => s.ImageUrl)
                .FirstOrDefaultAsync()) ?? string.Empty;
        }

        public async Task<DefaultUser> GetUserByIdAsync(string tgId)
        {
            return await _ctx.AllUsers.Where(s => s.TgId == tgId)
                .Include(u => u.UserPayMethods)
                .FirstAsync();
        }

        public async Task<long> GetUserIdByTgIdAsync(string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.AllUsers
                .Where(c => c.TgId == tgId)
                .Select(s => s.Id)
                .FirstAsync();
        }

        public async Task<GetUserProfile> GetUserProfileByTgIdAsync(string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.Users.Where(t => t.TgId == tgId)
                .ProjectTo<GetUserProfile>(_mapper.ConfigurationProvider)
                .FirstAsync();
            return result;
        }

        public async Task<Tuple<long, string>[]> MapUserIdsToTgIds(long[] ids)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.AllUsers
                .Where(s => ids.Contains(s.Id))
                .Select(s => Tuple.Create(s.Id, s.TgId))
                .ToArrayAsync();
            return result;
        }

        public async Task RemoveUserAsync(string tgId)
        {
            await _ctx.AllUsers.Where(u => u.TgId == tgId).ExecuteDeleteAsync();
        }
    }
}
