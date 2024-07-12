namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IAuth
    {
        public string GetToken(TelegramAuthDateDto dto);
    }
}
