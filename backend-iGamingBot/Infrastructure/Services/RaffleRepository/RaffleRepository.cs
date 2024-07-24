using AutoMapper;
using AutoMapper.QueryableExtensions;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure.Configs;
using backend_iGamingBot.Models;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Services.RaffleRepository
{
    public class RaffleRepository : IRaffleRepository
    {
        private readonly AppCtx _ctx;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<AppCtx> _factory;

        public RaffleRepository(AppCtx ctx, IDbContextFactory<AppCtx> factory, IMapper mapper) 
        {
            _ctx = ctx;
            _mapper = mapper;
            _factory = factory;
        }
        public async Task CreateRaffleAsync(Raffle raffle) => await _ctx.Raffles.AddAsync(raffle);

        public async Task<ParticipantNote> GetRaffleParticipantNote(long raffleId, long userId)
        {
            return await _ctx.Raffles
                .Where(r => r.Id == raffleId)
                .SelectMany(r => r.ParticipantsNote)
                .Where(n => n.ParticipantId == userId)
                .FirstAsync();
        }

        public async Task<long[]> GetParticipantsIdForRaffle(long raffleId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Raffles
                .Where(r => r.Id.Equals(raffleId))
                .SelectMany(r => r.Participants)
                .Select(u => u.Id)
                .ToArrayAsync();
        }

        public async Task<GetRaffleDto> GetRaffleByIdAsync(long id)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var raffles = ctx.Raffles.Where(r => r.Id.Equals(id));
            return await raffles.Select(r => new GetRaffleDto()
            {
                AmountOfParticipants = r.Participants.Count,
                AmountOfWinners = r.AmountOfWinners,
                Description = r.Description,
                EndTime = r.EndTime,
                Id = r.Id,
                RaffleConditions = r.RaffleConditions
            }).FirstAsync();
        }

        public async Task<GetSubscriberDto[]> GetRaffleWinners(long raffleId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var raffle = await _ctx.Raffles
                .Where(r => r.Id == raffleId)
                .Include(r => r.WinnersNote)
                .FirstAsync();
            var winnersId = raffle.WinnersNote.Select(u => u.WinnerId);
            var winnersWithMultipleWins = raffle.WinnersNote
                .Where(n => n.AmountOfWins > 1)
                .Select(n => new {n.AmountOfWins, n.WinnerId});
            var winners = await ctx.Subscribers
                .Where(s => s.StreamerId == raffle.CreatorId && winnersId.Contains(s.UserId))
                .ProjectTo<GetSubscriberDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
           foreach(var multipleWinner in winnersWithMultipleWins)
           {
                var winner = winners.First(w => w.Id == multipleWinner.WinnerId);
                var notesToAdd = multipleWinner.AmountOfWins - 1;
                while (notesToAdd > 0)
                {
                    var note = winner;
                    winners.Add(note);
                    notesToAdd--;
                }
           }
            return winners.ToArray();
        }

        public async Task<Raffle> GetTrackingRaffleByIdAsync(long id)
        {
            return await _ctx.Raffles
                .Where(r => r.Id == id)
                .Include(r => r.Creator)
                .FirstAsync();
        }

        public async Task<bool> UserAlreadyHaveWinRaffle(long raffleId, long userId)
        {
            return await _ctx.Raffles.Where(r => r.Id == raffleId)
                .SelectMany(r => r.WinnersNote)
                .Where(n => n.WinnerId == userId)
                .AnyAsync();
        }

        public async Task<bool> UserNotAbuseRaffle(long raffleId, long userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var payMethods = await ctx.AllUsers
                .Where(u => u.Id == userId)
                .SelectMany(u => u.UserPayMethods)
                .Where(p => p.Data != null)
                .ToArrayAsync();
            var payMethodsAdresses = payMethods.Select(p => p.Data);
            var amountOfUsersWithSamePayments = await ctx.Raffles
                .Where(r => r.Id != raffleId)
                .SelectMany(r => r.Participants)
                .Where(p => p.UserPayMethods.Select(p => p.Data).Where(a => payMethodsAdresses.Contains(a)).Any())
                .CountAsync();
            return amountOfUsersWithSamePayments == 1 || (amountOfUsersWithSamePayments == 0 && payMethodsAdresses.Count() == 0);
        }

        public async Task<WinnerNote[]> GetRaffleWinnerNotes(long raffleId, long[] userIds)
        {
            return await _ctx.Raffles
                .Where(r => r.Id == raffleId)
                .SelectMany(r => r.WinnersNote)
                .Where(n => userIds.Contains(n.WinnerId))
                .ToArrayAsync();
        }

        public async Task AddWinnerNote(WinnerNote winnerNote)
        => await _ctx.WinnerNotes.AddAsync(winnerNote);

        public async Task<long[]> GetRafflesAlreadyEnded()
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Raffles
                .Where(r => r.EndTime < DateTime.UtcNow && !r.WinnersDefined)
                .Select(r => r.Id)
                .ToArrayAsync();
        }

        public async Task<GetReportRaffleWinner[]> GetRaffleWinnersForReport(long id)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.Participants
                .Where(n => n.RaffleId == id)
                .ProjectTo<GetReportRaffleWinner>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return result;
        }
    }
}
