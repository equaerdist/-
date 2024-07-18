using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class StreamerService : IStreamerService
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserRepository _userSrc;
        private readonly IStreamerRepository _streamerSrc;
        private readonly IUserService _userSrv;

        public StreamerService(IUnitOfWork uof, IUserRepository userSrc, 
            IStreamerRepository streamerSrc,
            IUserService userSrv) 
        {
            _uof = uof;
            _userSrc = userSrc;
            _streamerSrc = streamerSrc;
            _userSrv = userSrv;
        }

        public async Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, string streamerId, string userId)
        {
            var pageResult = await _streamerSrc.GetRafflesAsync(page, pageSize, type, streamerId, userId);
            foreach (var result in pageResult)
            {
                foreach(var c in result.RaffleConditions)
                {
                    c.IsDone = await _userSrv.ConditionIsDone(c.Description, userId);
                }
            }
            return pageResult;
        }

        public async Task SubscribeToStreamerAsync(string streamerId, string userId)
        {
            var streamerEntry = await _userSrc.GetUserByIdAsync(streamerId);
            var userEntry = await _userSrc.GetUserByIdAsync(userId);
            if (!(streamerEntry is Streamer streamer))
                throw new InvalidOperationException();
            if(!(userEntry is User user))
                throw new InvalidOperationException();
            streamer.SubscribersRelation.Add(new() { User = user, SubscribeTime = DateTime.UtcNow });
            await _uof.SaveChangesAsync();
        }

        public Task UnscribeFromStreamerAsync(string streamerId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
