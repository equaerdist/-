using backend_iGamingBot.Dto;
using backend_iGamingBot.Models;
using Microsoft.AspNetCore.Http.Connections;
using System.Diagnostics.Eventing.Reader;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IRaffleRepository
    {
        public Task CreateRaffleAsync(Raffle raffle);
        public Task<GetRaffleDto> GetRaffleByIdAsync(long id);
        public Task<Raffle> GetTrackingRaffleByIdAsync(long id);
        public Task<GetSubscriberDto[]> GetRaffleWinners(long raffleId);
        public Task<long[]> GetParticipantsIdForRaffle(long raffleId);
        public Task<bool> UserNotAbuseRaffle(long raffleId, long userId);
        public Task<bool> UserAlreadyHaveWinRaffle(long raffleId, long userId);
        public Task<ParticipantNote> GetRaffleParticipantNote(long raffleId, long userId);
        public Task<WinnerNote[]> GetRaffleWinnerNotes(long raffleId, long[] userIds);
        public Task AddWinnerNote(WinnerNote winnerNote);
    }
}
