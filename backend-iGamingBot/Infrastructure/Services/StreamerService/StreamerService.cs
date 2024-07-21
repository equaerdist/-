using AutoMapper;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Models;
using System.Text.RegularExpressions;

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
        private readonly TelegramPostCreator _postsCreator;

        public StreamerService(IUnitOfWork uof, 
            IUserRepository userSrc, 
            IStreamerRepository streamerSrc,
            IUserService userSrv,
            IMapper mapper,
            IRaffleRepository raffleSrc,
            TelegramPostCreator postsCreator) 
        {
            _uof = uof;
            _userSrc = userSrc;
            _streamerSrc = streamerSrc;
            _userSrv = userSrv;
            _mapper = mapper;
            _raffleSrc = raffleSrc;
            _postsCreator = postsCreator;
        }
        private bool ValidateNewRaffle(CreateRaffleRequest request)
        {
            if (string.IsNullOrEmpty(request.Description) 
                || request.Description.Length < AppConfig.MinimalLengthForText)
                throw new AppException(AppDictionary.RaffleDescriptionNotEmpty);
            if (request.EndTime <= DateTime.UtcNow + AppDictionary.MinimalReserveForRaffleEnd)
                throw new AppException(AppDictionary.RaffleEndTimeTooSoon);
            if (request.AmountOfWinners < 1)
                throw new AppException(AppDictionary.RaffleAmountWinnersTooSmall);
            return true;
        }

        public async Task<Raffle> CreateRaffleAsync(CreateRaffleRequest request, string tgId, 
            string sourceId)
        {
            if (await _streamerSrc.GetAccessLevel(tgId, sourceId) == Access.None)
                throw new AppException(AppDictionary.NotHaveAccess);
            _ = ValidateNewRaffle(request);
            var userId = await _userSrc.GetUserIdByTgIdAsync(tgId);
            var raffle = _mapper.Map<Raffle>(request);
            raffle.CreatorId = userId;
            await _raffleSrc.CreateRaffleAsync(raffle);
            if(request.ShouldNotifyUsers)
            {
                var batchNum = 1;
                var streamer = await _streamerSrc.GetStreamerByTgIdAsync(tgId, "1");
                while (true)
                {
                    var batch = await _streamerSrc.GetBatchOfStreamerSubscribersAsync(tgId, 
                        AppConfig.USER_BATCH_SIZE, batchNum);
                    var post = new CreatePostRequest() 
                    { 
                        Message = $"У стримера {streamer.Name} начался розыгрыш!" 
                    };
                    _postsCreator.AddPostToLine((post, tgId, batch));
                    if (batch.Length < AppConfig.USER_BATCH_SIZE)
                        break;
                }
            }
            await _uof.SaveChangesAsync();
            return raffle;
        }

        public string[] GetAvailableSocials()
        {
           return AppDictionary.ResolvedSocialNames.Select(s => s.name).ToArray();
        }

        public async Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, 
            string streamerId, string userId)
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
            streamer.SubscribersRelation.Add(new() { User = userEntry, SubscribeTime = DateTime.UtcNow });
            await _uof.SaveChangesAsync();
        }

        public async Task UnscribeFromStreamerAsync(string streamerId, string userId)
        {
            await _streamerSrc.RemoveSubscribeRelationAsync(streamerId, userId);
        }
       
        public async Task CreatePostAsync(CreatePostRequest request, string tgId, string sourceId)
        {
            if (await _streamerSrc.GetAccessLevel(tgId, sourceId) == Access.None)
                throw new AppException(AppDictionary.Denied);
            Validators.ValidatePostRequest(request);
            var batchNum = 1;
            while(true)
            {
                var batch = await _streamerSrc.GetBatchOfStreamerSubscribersAsync(tgId, AppConfig.USER_BATCH_SIZE, batchNum);
                batchNum++;
                _postsCreator.AddPostToLine((request, tgId, batch));
                if (batch.Length < AppConfig.USER_BATCH_SIZE)
                    break;
            }
        }

        public async Task DoParticipantInRaffleAsync(long raffleId, string userId)
        {
            var raffle = await _raffleSrc.GetTrackingRaffleByIdAsync(raffleId);
            if (raffle.EndTime < DateTime.UtcNow)
                throw new AppException(AppDictionary.RaffleTimeExceeded);
            var user = await _userSrc.GetUserByIdAsync(userId);
            raffle.Participants.Add(user);
            await _uof.SaveChangesAsync();
        }

        public async Task AddStreamerSocial(GetSocialDto request, string streamerId, string sourceId)
        {
            if (await _streamerSrc.GetAccessLevel(streamerId, sourceId) == Models.Access.None)
                throw new AppException(AppDictionary.NotHaveAccess);
            var isResolved = false;
            foreach(var resolvedSocial in AppDictionary.ResolvedSocialNames)
            {
                isResolved = resolvedSocial.name == request.Name
                    && Regex.IsMatch(request.Link, resolvedSocial.pattern);
            }
            if (!isResolved)
                throw new AppException(AppDictionary.NotResolvedSocial);
            var streamer = (await _userSrc.GetUserByIdAsync(streamerId) as Streamer)!;
            if(streamer.Socials.Any(s => s.Name == request.Name))
                streamer.Socials.Remove(streamer.Socials.First(c => c.Name == request.Name));
            var socialToAdd = _mapper.Map<Social>(request);
            socialToAdd.Parameter = new();
            streamer.Socials.Add(socialToAdd);
           await _uof.SaveChangesAsync();
        }

        public async Task AddStreamerAdmin(string streamerId, string userId, string sourceId)
        {
            if(await _streamerSrc.GetAccessLevel(streamerId, sourceId) != Access.Full)
                throw new AppException(AppDictionary.Denied);
            var streamer = (await _userSrc.GetUserByIdAsync(streamerId) as Streamer)!;
            var newAdmin = await _userSrc.GetUserByIdAsync(userId);
            newAdmin.Negotiable.Add(streamer);
            await _uof.SaveChangesAsync();
        }
    }
}
