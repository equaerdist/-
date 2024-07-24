
namespace backend_iGamingBot.Infrastructure.Services
{
    public class RaffleService : IRaffleService
    {
        private readonly IUnitOfWork _uof;
        private readonly IRaffleRepository _rafleSrc;
        private readonly IStreamerRepository _streamerSrc;
        private readonly TelegramPostCreator _postCreator;

        public RaffleService(IRaffleRepository raffleSrc, 
            IUnitOfWork uof,
            IStreamerRepository streamerSrc,
            TelegramPostCreator postCreator) 
        {
            _uof = uof;
            _rafleSrc = raffleSrc;
            _streamerSrc = streamerSrc;
            _postCreator = postCreator;
        }
        public async Task GenerateWinnersForRaffle(long raffleId, bool exceptRepeat,
             string? sourceId = null,
            int? amountOfWinners = null)
        {
            var raffleTask =  _rafleSrc.GetTrackingRaffleByIdAsync(raffleId);
            var participantsTask = _rafleSrc.GetParticipantsIdForRaffle(raffleId);
            await Task.WhenAll(raffleTask, participantsTask);
            if (sourceId != null)
            {
                if (await _streamerSrc.GetAccessLevel(raffleTask.Result.Creator!.TgId, sourceId) == Models.Access.None)
                    throw new AppException(AppDictionary.Denied);
            }
            var aow = amountOfWinners ?? raffleTask.Result.AmountOfWinners;
            List<long> winners = new List<long>();
            int generatesTime = 0;
            List<long> multipleWinners = new();
            while(true)
            {
                generatesTime++;
                var participants = participantsTask.Result;
                if(participants.Length == 0)
                    break;
                var winnerId = participants[Random.Shared.Next(0, participants.Length)];
                
                if(!(await _rafleSrc.UserNotAbuseRaffle(raffleId, winnerId)))
                {
                    var noteAboutParticipant = await _rafleSrc.GetRaffleParticipantNote(raffleId, winnerId);
                    noteAboutParticipant.HaveAbused = true;
                    continue;
                }
                if (await _rafleSrc.UserAlreadyHaveWinRaffle(raffleId, winnerId))
                {
                    if (exceptRepeat)
                    { 
                        continue; 
                    }
                    else
                    {
                        multipleWinners.Add(winnerId);
                    }
                }
                winners.Add(winnerId);
                if (winners.Count == aow || generatesTime > aow * 4)
                    break;
            }
            if (!exceptRepeat && multipleWinners.Count != 0)
            {
                var multipleWinnerNotes = await _rafleSrc.GetRaffleWinnerNotes(raffleId, multipleWinners.ToArray());
                foreach (var multipleWinnerNote in multipleWinnerNotes)
                {
                    var amountOfWins = multipleWinners.Count(t => t == multipleWinnerNote.WinnerId);
                    multipleWinnerNote.AmountOfWins += amountOfWins;
                }
            }
            var singleWinners = winners.Except(multipleWinners);
            foreach (var singleWinner in singleWinners)
            {
                await _rafleSrc.AddWinnerNote(new()
                {
                    AmountOfWins = 1,
                    RaffleId = raffleId,
                    WinnerId = singleWinner
                });
            }
            var postReq = new TelegramPostRequest()
            {
                Message = $"Вы выиграли в розыгрыше стримера {raffleTask.Result.Creator!.Name}",
                StreamerId = raffleTask.Result.Creator.TgId,
                Viewers = winners.ToArray()
            };
            _postCreator.AddPostToLine(postReq);
            raffleTask.Result.WinnersDefined = true;
            await _uof.SaveChangesAsync();
        }
    }
}
