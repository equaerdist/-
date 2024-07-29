
using Telegram.Bot;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class TelegramExtensions : ITelegramExtensions
    {
        private readonly ITelegramBotClient _botClient;
        private readonly AppConfig _cfg;

        public TelegramExtensions(ITelegramBotClient botClient, AppConfig cfg) 
        { 
            _botClient = botClient;
            _cfg = cfg;
        }

        public async Task<string?> GetUserImageUrl(long id)
        {
            var userPhotos = await _botClient.GetUserProfilePhotosAsync(id, limit: 1);
            var firstFileId = userPhotos.Photos.SelectMany(s => s).FirstOrDefault()?.FileId;
            string? filePath = null;
            if (firstFileId != null)
            {
                var file = await _botClient.GetFileAsync(firstFileId);
                filePath = $"{_cfg.TgFilePath}{_cfg.TgKey}/{file.FilePath}";
            }
            return filePath;
        }
    }
}
