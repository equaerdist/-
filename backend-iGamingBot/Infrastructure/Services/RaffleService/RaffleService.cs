﻿
namespace backend_iGamingBot.Infrastructure.Services
{
    public class RaffleService : IRaffleService
    {
        private readonly IUnitOfWork _uof;
        private readonly IRaffleRepository _rafleSrc;

        public RaffleService(IRaffleRepository raffleSrc, IUnitOfWork uof) 
        {
            _uof = uof;
            _rafleSrc = raffleSrc;
        }
        public async Task GenerateWinnersForRaffle(long raffleId, bool exceptRepeat, 
            int? amountOfWinners = null)
        {
            var raffleTask =  _rafleSrc.GetRaffleByIdAsync(raffleId);
            var participantsTask = _rafleSrc.GetParticipantsIdForRaffle(raffleId);
            await Task.WhenAll(raffleTask, participantsTask);
            var aow = amountOfWinners ?? raffleTask.Result.AmountOfWinners;
            List<long> winners = new List<long>();
            int generatesTime = 0;
            List<long> multipleWinners = new();
            while(true)
            {
                generatesTime++;
                var participants = participantsTask.Result;
                var winnerId = participants[Random.Shared.Next(0, participants.Length)];
                
                if(!(await _rafleSrc.UserNotAbuseRaffle(raffleId, winnerId)))
                {
                    var noteAboutParticipant = await _rafleSrc.GetRaffleParticipantNote(raffleId, winnerId);
                    noteAboutParticipant.HaveAbused = true;
                    continue;
                }
                if (exceptRepeat)
                {
                    if (await _rafleSrc.UserAlreadyHaveWinRaffle(raffleId, winnerId))
                        continue;
                }
                else 
                    multipleWinners.Add(winnerId);
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
            await _uof.SaveChangesAsync();
        }
    }
}
