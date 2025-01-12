﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using backend_iGamingBot.Dto;
using backend_iGamingBot.Infrastructure.Configs;
using backend_iGamingBot.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class StreamerRepository : IStreamerRepository
    {
        private readonly AppCtx _ctx;
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<AppCtx> _factory;

        public StreamerRepository(AppCtx ctx, 
            IDbContextFactory<AppCtx> factory,
            IMapper mapper) 
        {
            _ctx = ctx;
            _mapper = mapper;
            _factory = factory;
        }

        public async Task<GetAdminDto[]> GetAdminsAsync(string tgId)
        {
           using var ctx = await _factory.CreateDbContextAsync();
           var result = await ctx.AllUsers
                .Where(u => u.Negotiable.Select(u => u.TgId).Contains(tgId))
                .ProjectTo<GetAdminDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return result;
        }

        public async Task<long[]> GetBatchOfStreamerSubscribersAsync(string streamerTgId, 
            int batchSize, int num)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var batch = await ctx.Streamers
                .Where(s => s.TgId == streamerTgId)
                .SelectMany(s => s.Subscribers)
                .Select(u => u.TgId)
                .OrderBy(u => u)
                .Skip((num - 1) * batchSize)
                .Take(batchSize)
                .ToArrayAsync();
            return batch.Select(s => long.Parse(s)).ToArray();
        }

        public async Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, string tgId, string userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            Expression<Func<Raffle, bool>> filter = type == "active" ?
                r => r.EndTime > DateTime.UtcNow : r => r.EndTime <= DateTime.UtcNow && r.WinnersDefined;
            var raffles = ctx.Raffles.Where(r => r.Creator!.TgId == tgId);
            raffles = raffles.Where(filter)
                .OrderBy(r => r.EndTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
            return await raffles.Select(r => new GetRaffleDto()
            {
                AmountOfParticipants = r.Participants.Count,
                AmountOfWinners = r.AmountOfWinners,
                Description = r.Description,
                EndTime = r.EndTime,
                Id = r.Id,
                IsParticipant = r.Participants.Select(u => u.TgId).Contains(userId),
                IsCreator = r.Creator!.TgId.Equals(userId),
                RaffleConditions = r.RaffleConditions
            }).ToArrayAsync();
        }

        public async Task<Streamer[]> GetStreamerBatchAsync(int page, int pageSize)
        {
            var batch = await _ctx.Streamers
                .OrderBy(s => s.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();
            return batch;
        }

        public async Task<GetStreamerDto> GetStreamerByTgIdAsync(string tgId, string userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var streamer = await ctx.Streamers
                .Where(s => s.TgId.Equals(tgId))
                .Select(t => new GetStreamerDto()
                {
                    AmountOfSubscribers = t.Subscribers.Count(),
                    Id = t.Id,
                    IsSubscribed = t.Subscribers.Select(t => t.TgId).Contains(userId),
                    TgId = t.TgId,
                    Name = t.Name,
                    Socials = t.Socials,
                    ImageUrl = UserResolver.ExtractFilePath(t.ImageUrl),
                })
                .FirstAsync();
            streamer.IsLive = streamer.Socials.Any(s => s.Parameter.IsLive);
            return streamer;
        }

        public async Task<GetSocialDto[]> GetStreamerSocials(string streamerId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var streamer = await ctx.Streamers
                .Where(s => s.TgId == streamerId)
                .FirstAsync();
            return _mapper.Map<GetSocialDto[]>(streamer.Socials);
        }

        public async Task<GetStreamerDto[]> GetStreamersPageAsync(int page, int pageSize, string userId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var streamersPage = await ctx.Streamers
                .OrderBy(s => s.Subscribers.Select(t => t.TgId).Contains(userId))
                .OrderBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new GetStreamerDto()
                {
                    AmountOfSubscribers = t.Subscribers.Count(),
                    Id = t.Id,
                    IsSubscribed = t.Subscribers.Select(t => t.TgId).Contains(userId),
                    TgId = t.TgId,
                    Name = t.Name,
                    ImageUrl = UserResolver.ExtractFilePath(t.ImageUrl),
                    Socials = t.Socials,
                })
                .ToArrayAsync();
            foreach (var streamer in streamersPage)
                streamer.IsLive = streamer.Socials.Any(s => s.Parameter.IsLive);
            return streamersPage;
        }

        public async Task<GetSubscriberDto[]> GetSubscribersAsync(int page, int pageSize, string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            var result = await ctx.Subscribers
                .Where(s =>  s.Streamer!.TgId.Equals(tgId))
                .OrderByDescending(s => s.SubscribeTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<GetSubscriberDto>(_mapper.ConfigurationProvider)
                .ToArrayAsync();
            return result;
        }

        public async Task RemoveSubscribeRelationAsync(string streamerId, string userId)
        {
            await _ctx.Subscribers.Where(c => c.Streamer!.TgId.Equals(streamerId) && c.User!.TgId.Equals(userId))
                .ExecuteDeleteAsync();
        }

        public async Task<Access> GetAccessLevel(string targetId, string sourceId)
        {
            if (targetId == sourceId)
                return Access.Full;
            using var ctx = await _factory.CreateDbContextAsync();
            if (await ctx.AllUsers
                .Where(s => s.TgId == sourceId)
                .SelectMany(u => u.Negotiable)
                .AnyAsync(s => s.TgId == targetId))
                return Access.Admin;
            return Access.None;
        }

        public async Task<Streamer> GetStreamerByName(string name)
        {
            var streamer = await _ctx.Streamers
                .AsNoTracking()
                .Where(s => s.Name == name)
                .FirstAsync();
            return streamer;
        }

        public async Task<string> GetStreamerNameByTgId(string tgId)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.Streamers
                .Where(s => s.TgId.Equals(tgId))
                .Select(s => s.Name)
                .FirstAsync();
        }

        public async Task CreateStreamerInvite(StreamerInvite invite) => await _ctx.StreamerInvites.AddAsync(invite);

        public async Task<bool> StreamerInviteAlreadyExists(string inviteCode)
        {
            using var ctx = await _factory.CreateDbContextAsync();
            return await ctx.StreamerInvites
                .Where(s => s.Name + "-" + s.Code.ToString() == inviteCode)
                .AnyAsync();
        }

        public async Task RemoveStreamerInvite(string name)
        {
            await _ctx.StreamerInvites.Where(s => s.Name.Equals(name)).ExecuteDeleteAsync();
        }

        public async Task RemoveAdminInvite(string name, Guid code) => 
            await _ctx.AdminInvites.Where(s => s.Name == name && s.Code == code)
            .ExecuteDeleteAsync();

        public async Task CreateAdminInvite(AdminInvite adminInvite) => 
            await _ctx.AdminInvites.AddAsync(adminInvite);

        public async Task<AdminInvite> GetAdminInvite(string name, Guid code)
        {
            return await _ctx.AdminInvites
             .Where(s => s.Name == name && s.Code == code)
             .FirstAsync();
        }

        public void RemoveAdminInvite(AdminInvite adminInvite)
        {
           _ctx.AdminInvites.Remove(adminInvite);
        }
    }
}
