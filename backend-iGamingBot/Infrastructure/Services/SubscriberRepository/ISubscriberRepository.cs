using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface ISubscriberRepository
    {
        public Task<GetSubscriberProfile> GetSubProfileByTgId(string id, string streamerId);
        public Task<Subscriber> GetSubscriberByTgId(string tgId, string streamerId);
    }
}
