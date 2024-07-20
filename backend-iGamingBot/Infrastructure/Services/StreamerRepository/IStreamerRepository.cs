using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IStreamerRepository
    {
        public Task<Streamer[]> GetStreamerBatchAsync(int page, int pageSize);
        public Task<GetStreamerDto> GetStreamerByTgIdAsync(string tgId, string userId);
        public Task<GetStreamerDto[]> GetStreamersPageAsync(int page, int pageSize, string userId);
        public Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, string tgId, string userId);
        public Task<GetSubscriberDto[]> GetSubscribersAsync(int page, int pageSize, string tgId);
        public Task<GetAdminDto[]> GetAdminsAsync(string tgId);
        public Task RemoveSubscribeRelationAsync(string streamerId, string userId);
        public Task<long[]> GetBatchOfStreamerSubscribersAsync(string streamerTgId, 
            int batchSize, int num);
        public Task<GetSocialDto[]> GetStreamerSocials(string streamerId);
    }
}
