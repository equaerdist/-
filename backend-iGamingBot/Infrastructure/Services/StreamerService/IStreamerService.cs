﻿using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IStreamerService
    {
        public Task SubscribeToStreamerAsync(string streamerId, string userId);
        public Task UnscribeFromStreamerAsync(string streamerId, string userId);
        public Task<GetRaffleDto[]> GetRafflesAsync(int page, int pageSize, string type, string streamerId,
            string userId);
        public string[] GetAvailableSocials();
        public  Task<Raffle> CreateRaffleAsync(CreateRaffleRequest request, string tgId);
        public Task CreatePostAsync(CreatePostRequest request, string tgId);
    }
}
