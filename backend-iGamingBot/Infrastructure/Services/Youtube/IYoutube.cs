using backend_iGamingBot.Infrastructure.Services;

namespace backend_iGamingBot.Infrastructure
{
    public interface IYoutube
    {
        public Task<string> GetUserIdentifierByLinkAsync(string link);
        public Task<StreamInformation> UserIsStreaming(string identifier);
    }
}
