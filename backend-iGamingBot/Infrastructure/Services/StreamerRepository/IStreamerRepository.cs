using backend_iGamingBot.Dto;
using backend_iGamingBot.Models;

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
        public Task<Access> GetAccessLevel(string targetId, string sourceId);
        public Task<Streamer> GetStreamerByName(string name);
        public Task<string> GetStreamerNameByTgId(string tgId);
        public Task CreateStreamerInvite(StreamerInvite invite);
        public Task<bool> StreamerInviteAlreadyExists(string inviteCode);
        public Task RemoveStreamerInvite(string name);
        public Task RemoveAdminInvite(string name, Guid code);
        public Task CreateAdminInvite(AdminInvite adminInvite);
        public Task<AdminInvite> GetAdminInvite(string name, Guid code);
        public void RemoveAdminInvite(AdminInvite adminInvite);
    }
}
