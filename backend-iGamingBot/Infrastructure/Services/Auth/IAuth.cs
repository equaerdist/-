namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IAuth
    {
        public Task<string> GetTokenAsync(TelegramAuthDateDto dto);
    }
}
