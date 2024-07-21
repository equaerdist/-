using backend_iGamingBot.Dto;

namespace backend_iGamingBot.Infrastructure.Services
{ 
    public interface ISubscriberService
    {
        public Task SendSubMessage(SendSubMessageRequest req, string sourceId);
        public Task EditNoteAboutSub(EditNoteAboutSub req, string sourceId);
    }
}
