namespace backend_iGamingBot.Infrastructure.Services
{
    public interface ITelegramExtensions
    {
        public Task<string?> GetUserImageUrl(long id);
    }
}
