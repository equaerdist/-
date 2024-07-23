using backend_iGamingBot.Dto;
using backend_iGamingBot.Models;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class SubscriberService : ISubscriberService
    {
        private readonly TelegramPostCreator _postCreator;
        private readonly IStreamerRepository _streamerSrc;
        private readonly ISubscriberRepository _subSrc;
        private readonly IUnitOfWork _uof;

        public SubscriberService(TelegramPostCreator postCreator, 
            IStreamerRepository streamerSrc,
            ISubscriberRepository subSrc,
            IUnitOfWork uof) 
        { 
            _postCreator = postCreator;
            _streamerSrc = streamerSrc;
            _subSrc = subSrc;
            _uof = uof;
        }

        public async Task EditNoteAboutSub(EditNoteAboutSub req, string sourceId)
        {
            if (await _streamerSrc.GetAccessLevel(req.StreamerId, sourceId) == Access.None)
                throw new AppException(AppDictionary.Denied);
            var sub = await _subSrc.GetSubscriberByTgId(req.Id, req.StreamerId);
            sub.Note = req.Note;
            await _uof.SaveChangesAsync();
        }

        public async Task SendSubMessage(SendSubMessageRequest req, string sourceId)
        {
            if (await _streamerSrc.GetAccessLevel(req.StreamerId, sourceId) != Access.Full)
                throw new AppException(AppDictionary.NotHaveAccess);
            var streamer = await _streamerSrc.GetStreamerByTgIdAsync(req.StreamerId, req.Id);
            var message = new TelegramPostRequest() 
            { 
                Message = $"Вам пришло сообщение от стримера {streamer.Name}\n" +
                $"{req.Message}" ,
                Media = null,
                Viewers = [long.Parse(req.Id)],
                StreamerId = req.StreamerId
            };
            Validators.ValidatePostRequest(new() { Message = req.Message });
            _postCreator.AddPostToLine(message);
        }
    }
}
