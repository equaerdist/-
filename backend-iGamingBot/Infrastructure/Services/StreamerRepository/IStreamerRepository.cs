namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IStreamerRepository
    {
        public Task<Streamer[]> GetStreamerBatchAsync(int page, int pageSize);
    }
}
