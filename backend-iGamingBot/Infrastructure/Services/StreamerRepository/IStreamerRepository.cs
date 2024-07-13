using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IStreamerRepository
    {
        public Task<Streamer[]> GetStreamerBatchAsync(int page, int pageSize);
        public Task<GetStreamerDto> GetStreamerByTgIdAsync(string tgId);
        public Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, long streamerId);
        public Task<GetSubscriberDto[]> GetSubscribersAsync(int page, int pageSize, long id);
    }
}
