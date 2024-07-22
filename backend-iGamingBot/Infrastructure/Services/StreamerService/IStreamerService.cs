using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IStreamerService
    {
        public Task SubscribeToStreamerAsync(string streamerId, string userId);
        public Task UnscribeFromStreamerAsync(string streamerId, string userId);
        public Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, string streamerId,
            string userId);
        public string[] GetAvailableSocials();
        public  Task<Raffle> CreateRaffleAsync(CreateRaffleRequest request, string tgId, string sourceId);
        public Task CreatePostAsync(CreatePostRequest request, string tgId, string sourceId);
        public Task DoParticipantInRaffleAsync(long raffleId, string userId);
        public Task AddStreamerSocial(GetSocialDto request, string streamerId, string sourceId);
        public Task AddStreamerAdmin(string streamerId, string userId, string sourceId);
        public Task CreateRequestForRaffleReport(long id, string sourceId);
        public Task CreateRequestForSubscribersReport(string streamerId, string sourceId);
    }
}
