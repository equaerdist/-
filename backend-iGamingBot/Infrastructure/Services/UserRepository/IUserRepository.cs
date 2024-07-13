namespace backend_iGamingBot.Infrastructure.Services.UserRepository
{
    public interface IUserRepository
    {
        public Task<string> DefineRoleByTgIdAsync(string tgId);
    }
}
