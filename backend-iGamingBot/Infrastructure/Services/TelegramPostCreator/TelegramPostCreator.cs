using backend_iGamingBot.Dto;
using System.Collections.Concurrent;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class TelegramPostCreator : BackgroundService
    {
        private ITelegramBotClient _botClient = null!;
        private ILogger<TelegramPostCreator> _logger = null!;
        private readonly IServiceProvider _services;

        public async Task SendFileAsync(long chatId, IFormFile? file, string? caption = null)
        {
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                using var stream = file.OpenReadStream();

                switch (extension)
                {
                    case ".jpg":
                    case ".jpeg":
                    case ".png":
                    case ".gif":
                        await _botClient.SendPhotoAsync(
                            chatId: chatId,
                            photo: InputFile.FromStream(stream),
                            caption: caption,
                            parseMode: ParseMode.Markdown
                        );
                        break;

                    case ".mp4":
                        await _botClient.SendVideoAsync(
                            chatId: chatId,
                            video: InputFile.FromStream(stream),
                            caption: caption,
                            parseMode: ParseMode.Markdown
                        );
                        break;

                    default:
                        await _botClient.SendDocumentAsync(
                            chatId: chatId,
                            document: InputFile.FromStream(stream),
                            caption: caption,
                            parseMode: ParseMode.Markdown
                        );
                        break;
                }
            }
            else
            {
                if(caption != null)
                    await _botClient.SendTextMessageAsync(chatId:chatId, text: caption);
            }
        }
        private static ConcurrentQueue<(CreatePostRequest body, string streamerId, long[] viewers)> activeRequests =
            new ();
        public TelegramPostCreator(IServiceProvider services) 
        {
            _services = services;
        }
        public void AddPostToLine((CreatePostRequest body, string streamerId, long[] viewers) req)
        {
            activeRequests.Append(req);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(async () =>
        {
            var scope = _services.CreateScope();
            _botClient =  scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<TelegramPostCreator>>();
            while(true)
            {
                try
                {
                    if (AppConfig.Environment == AppConfig.LOCAL)
                        continue;
                    if (activeRequests.Count == 0)
                        continue;
                    if (!activeRequests.TryDequeue(out var request))
                        continue;
                    
                    while (true)
                    {
                        foreach (var subscriber in request.viewers)
                        {
                            try
                            {
                                await SendFileAsync(subscriber, request.body.Media, request.body.Message);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Произошла ошибка при отправке поста " +
                                    $"подписчику {subscriber} для стримера {request.streamerId}\n" +
                                    $"{ex.Message}");
                                continue;
                            }
                            await Task.Delay(AppConfig.DELAY_PER_REQUEST);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Произошла ошибка в расыльщике постов, начинаем заново\n{ex.Message}");
                    continue;
                }
            }
        });
    }
}
