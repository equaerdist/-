using AutoMapper;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly IYoutube _ytSrv;
        private readonly IExcelReporter _xlRep;
        private static string _inviteNameConstraint = "IX_Invites_Name";

        public StreamerService(IUnitOfWork uof, 
            IUserRepository userSrc, 
            IStreamerRepository streamerSrc,
            IUserService userSrv,
            IMapper mapper,
            IRaffleRepository raffleSrc,
            TelegramPostCreator postsCreator,
            IYoutube ytSrv,
            IExcelReporter xlRep) 
        {
            _uof = uof;
            _userSrc = userSrc;
            _streamerSrc = streamerSrc;
            _userSrv = userSrv;
            _mapper = mapper;
            _raffleSrc = raffleSrc;
            _postsCreator = postsCreator;
            _ytSrv = ytSrv;
            _xlRep = xlRep;
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
                    var post = new TelegramPostRequest() 
                    { 
                        Message = $"У стримера {streamer.Name} начался розыгрыш!",
                        StreamerId = tgId,
                        Viewers = batch,
                    };
                    _postsCreator.AddPostToLine(post);
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
                var resultConditions = new List<GetRaffleConditionDto>();
                foreach(var c in (List<string>)result.RaffleConditions)
                {
                    var condition = (string)c;
                    resultConditions.Add(new GetRaffleConditionDto()
                    {
                        Title = condition,
                        Description = AppDictionary.ResolvedConditions
                        .First(c => c.title.Equals(condition)).description,
                        IsDone = await _userSrv.SingleRaffleConditionIsDone(condition, userId)
                    });
                }
                result.RaffleConditions = resultConditions;
            }
            return pageResult;
        }

        public async Task SubscribeToStreamerAsync(string streamerId, string userId, string sourceId)
        {
            if (userId != sourceId) throw new AppException(AppDictionary.Denied);
            var streamerEntry = await _userSrc.GetUserByIdAsync(streamerId);
            var userEntry = await _userSrc.GetUserByIdAsync(userId);
            if (!(streamerEntry is Streamer streamer))
                throw new InvalidOperationException();
            streamer.SubscribersRelation.Add(new() { User = userEntry, SubscribeTime = DateTime.UtcNow });
            await _uof.SaveChangesAsync();
        }

        public async Task UnscribeFromStreamerAsync(string streamerId, string userId, string sourceId)
        {
            if (userId != sourceId) throw new AppException(AppDictionary.Denied);
            await _streamerSrc.RemoveSubscribeRelationAsync(streamerId, userId);
        }
       
        public async Task CreatePostAsync(CreatePostRequest request, string tgId, string sourceId)
        {
            if (await _streamerSrc.GetAccessLevel(tgId, sourceId) == Access.None)
                throw new AppException(AppDictionary.Denied);
            Validators.ValidatePostRequest(request);
            var streamerName = await _streamerSrc.GetStreamerNameByTgId(tgId);
            var batchNum = 1;
            while(true)
            {
                var batch = await _streamerSrc.GetBatchOfStreamerSubscribersAsync(tgId, 
                    AppConfig.USER_BATCH_SIZE, batchNum);
                batchNum++;
                PostCreatorFile? postFile = null;
                if(request.Media != null)
                {
                    postFile = new PostCreatorFile() { Stream = new(), Name = request.Media.FileName };
                    await request.Media.CopyToAsync(postFile.Stream);
                    postFile.Stream.Seek(0, SeekOrigin.Begin);
                }
                var postReq = new TelegramPostRequest()
                { 
                    Viewers = batch,
                    Media = postFile,
                    StreamerId = tgId,
                    Message = $"ПОСТ ОТ СТРИМЕРА {streamerName}:\n{request.Message}",
                };

                _postsCreator.AddPostToLine(postReq);
                if (batch.Length < AppConfig.USER_BATCH_SIZE)
                    break;
            }
        }

        public async Task DoParticipantInRaffleAsync(long raffleId, string userId)
        {
            var raffle = await _raffleSrc.GetTrackingRaffleByIdAsync(raffleId);
            if (raffle.EndTime < DateTime.UtcNow)
                throw new AppException(AppDictionary.RaffleTimeExceeded);
            var alreadyParticipantTask = _raffleSrc.UserAlreadyParticipant(userId, raffleId);
            var user = await _userSrc.GetUserByIdAsync(userId);
            var checkResult = await _userSrv.RaffleConditionIsDone(raffleId, userId);
            await alreadyParticipantTask;
            if(checkResult.Length != 0)
                throw new AppException(string.Join("\n", checkResult));
            if (alreadyParticipantTask.Result)
                throw new AppException(AppDictionary.AlreadyParticipant);
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
                    && Regex.IsMatch(request.Link, resolvedSocial.pattern, RegexOptions.IgnoreCase);
                if (isResolved)
                    break;
            }
            if (!isResolved)
                throw new AppException(AppDictionary.NotResolvedSocial);
            var streamer = (await _userSrc.GetUserByIdAsync(streamerId) as Streamer)!;
            if(streamer.Socials.Any(s => s.Name == request.Name))
                streamer.Socials.Remove(streamer.Socials.First(c => c.Name == request.Name));
            var socialToAdd = _mapper.Map<Social>(request);
            socialToAdd.Parameter = new();
            if (request.Name == AppDictionary.Youtube)
                socialToAdd.Parameter.Identifier = await _ytSrv.GetUserIdentifierByLinkAsync(request.Link);
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

        public async Task CreateRequestForRaffleReport(long id, string sourceId)
        {
            var raffle = await _raffleSrc.GetTrackingRaffleByIdAsync(id);
            if (await _streamerSrc.GetAccessLevel(raffle.Creator!.TgId, sourceId) == Access.None)
                throw new AppException(AppDictionary.Denied);
            var raffleWinners = await _raffleSrc.GetRaffleWinnersForReport(id);
            var file = await _xlRep.GenerateExcel(raffleWinners.ToList());
            var postReq = new TelegramPostRequest()
            {
                Message = $"Статистика по розыгрышу {id} от стримера {raffle.Creator!.Name}",
                Media = file,
                StreamerId = raffle.Creator!.TgId,
                Viewers = [long.Parse(sourceId)]
            };
            _postsCreator.AddPostToLine(postReq);
        }

        public async Task CreateRequestForSubscribersReport(string streamerId, string sourceId)
        {
            if(await _streamerSrc.GetAccessLevel(streamerId, sourceId) == Access.None)
                throw new AppException(AppDictionary.Denied);
            var pageNum = 1;
            var allUsers = new List<GetSubscriberDto>();
            while(true)
            {
                var page = await _streamerSrc.GetSubscribersAsync(pageNum, 
                    AppConfig.USER_BATCH_SIZE, 
                    streamerId);
                allUsers.AddRange(page);
                pageNum++;
                if (page.Length < AppConfig.USER_BATCH_SIZE)
                    break;
            }
            var streamerName = await _streamerSrc.GetStreamerNameByTgId(streamerId);
            var fileReport = await _xlRep.GenerateExcel(allUsers);
            var postReq = new TelegramPostRequest()
            { 
                StreamerId = streamerId,
                Viewers = [long.Parse(sourceId)],
                Message = $"Статистика по подписчикам для {streamerName}",
                Media = fileReport
            };

            _postsCreator.AddPostToLine(postReq);
        }

        public async Task RemoveStreamerAdmin(string streamerId, string userId, string sourceId)
        {
            if (await _streamerSrc.GetAccessLevel(streamerId, sourceId) != Access.Full)
                throw new AppException(AppDictionary.Denied);
            var streamer = (await _userSrc.GetUserByIdAsync(streamerId) as Streamer)!;
            var newAdmin = await _userSrc.GetUserByIdAsync(userId);
            newAdmin.Negotiable.Remove(streamer);
            await _uof.SaveChangesAsync();
        }

        public async Task<string> CreateStreamerInvite(string name)
        {
            try
            {
                await _streamerSrc.GetStreamerByName(name);
                throw new AppException(AppDictionary.UserAlreadyExists);
            }
            catch (InvalidOperationException)
            {
                StreamerInvite invite = new()
                {
                    Name = name,
                    Code = Guid.NewGuid()
                };
                await _streamerSrc.CreateStreamerInvite(invite);
                try
                {
                    await _uof.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                when (e.InnerException != null)
                {
                    if (e.InnerException.Message.Contains(_inviteNameConstraint))
                    {
                        await _streamerSrc.RemoveStreamerInvite(name);
                        await _uof.SaveChangesAsync();
                    }
                    else
                    {
                        throw new AppException(AppDictionary.StreamerAlreadyExists);
                    }
                }
                return $"{name}-{invite.Code}";
            }
        }
    }
}
