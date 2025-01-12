﻿namespace backend_iGamingBot.Dto
{
    public class CreateUserRequest
    {
        public string TgId { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? ImageUrl { get; set; }
        public string? Username { get; set; }
    }
}
