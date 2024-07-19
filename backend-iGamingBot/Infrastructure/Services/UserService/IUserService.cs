using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{ 
    public interface IUserService
    {
        public Task<bool> ConditionIsDone(string description, string userId);
        public Task<Streamer> RegisterStreamer(CreateStreamerRequest req);
        public Task<User> RegisterUser(CreateUserRequest req);
        public Task CheckUserInformation(CreateUserRequest req);
    }
}
