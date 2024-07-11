namespace backend_iGamingBot.Infrastructure
{
    public class AppException : Exception
    {
        public AppException(string? message) : base(message)
        {
        }
    }
}
