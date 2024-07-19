using AutoMapper;
using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class StreamerService : IStreamerService
    {
        private readonly IUnitOfWork _uof;
        private readonly IUserRepository _userSrc;
        private readonly IStreamerRepository _streamerSrc;
        private readonly IUserService _userSrv;
        private readonly IMapper _mapper;
        private readonly IRaffleRepository _raffleSrc;

        public StreamerService(IUnitOfWork uof, 
            IUserRepository userSrc, 
            IStreamerRepository streamerSrc,
            IUserService userSrv,
            IMapper mapper,
            IRaffleRepository raffleSrc) 
        {
            _uof = uof;
            _userSrc = userSrc;
            _streamerSrc = streamerSrc;
            _userSrv = userSrv;
            _mapper = mapper;
            _raffleSrc = raffleSrc;
        }

        public async Task<Raffle> CreateRaffleAsync(CreateRaffleRequest request, string tgId)
        {
            var userId = await _userSrc.GetUserIdByTgIdAsync(tgId);
            var raffle = _mapper.Map<Raffle>(request);
            raffle.CreatorId = userId;
            await _raffleSrc.CreateRaffleAsync(raffle);
            await _uof.SaveChangesAsync();
            return raffle;
        }

        public string[] GetAvailableSocials()
        {
           return AppDictionary.ResolvedSocialNames.Select(s => s.name).ToArray();
        }

        public async Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, string streamerId, string userId)
        {
            var pageResult = await _streamerSrc.GetRafflesAsync(page, pageSize, type, streamerId, userId);
            foreach (var result in pageResult)
            {
                var resultConditions = new List<object>();
                foreach(var c in result.RaffleConditions)
                {
                    var condition = (string)c;
                    resultConditions.Add(new GetRaffleConditionDto()
                    {
                        Title = condition,
                        Description = AppDictionary.ResolvedConditions
                        .First(c => c.title.Equals(condition)).description,
                        IsDone = await _userSrv.ConditionIsDone(condition, userId)
                    });
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

        public async Task UnscribeFromStreamerAsync(string streamerId, string userId)
        {
            await _streamerSrc.RemoveSubscribeRelationAsync(streamerId, userId);
        }
    }
}
