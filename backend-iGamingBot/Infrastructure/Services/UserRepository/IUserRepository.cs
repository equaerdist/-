using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IUserRepository
    {
        public Task<string> DefineRoleByTgIdAsync(string tgId);
        public Task<DefaultUser> GetUserByIdAsync(string tgId);
        public Task<long> GetUserIdByTgIdAsync(string tgId);
        public Task AddUserAsync(DefaultUser user);
        public Task<GetUserProfile> GetUserProfileByTgIdAsync(string tgId);
       
    }
}
