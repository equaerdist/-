using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface ISubscriberRepository
    {
        public Task<GetSubscriberProfile> GetSubProfileByTgId(string id, string streamerId);
    }
}
