namespace backend_iGamingBot.Infrastructure.Services
{ 
    public interface IUserService
    {
        public Task<bool> ConditionIsDone(string description, string userId);
    }
}
