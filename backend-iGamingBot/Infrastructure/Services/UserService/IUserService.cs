using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{ 
    public interface IUserService
    {
        public Task<string[]> RaffleConditionIsDone(long raffleId, string userId);
        public Task<bool> SingleRaffleConditionIsDone(string description, string userId);
        public Task<Streamer> RegisterStreamer(CreateStreamerRequest req);
        public Task<User> RegisterUser(CreateUserRequest req);
        public Task CheckUserInformation(CreateUserRequest req);
        public Task UpdateUserData(GetUserProfile dto, string sourceId);
    }
}
