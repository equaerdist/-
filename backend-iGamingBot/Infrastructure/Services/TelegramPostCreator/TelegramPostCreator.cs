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
        private AppConfig _cfg = null!;
        private readonly IServiceProvider _services;

        public async Task SendFileAsync(long chatId, PostCreatorFile? file, string? caption = null)
        {
            if (file != null)
            {
                var extension = Path.GetExtension(file.Name).ToLowerInvariant();
                var stream = file.Stream;
                var fileName = file.Name;
                using (file.Stream)
                {
                    switch (extension)
                    {
                        case ".jpg":
                        case ".jpeg":
                        case ".png":
                        case ".gif":
                            await _botClient.SendPhotoAsync(
                                chatId: chatId,
                                photo: InputFile.FromStream(stream, fileName),
                                caption: caption,
                                parseMode: ParseMode.Markdown
                            );
                            break;

                        case ".mp4":
                            await _botClient.SendVideoAsync(
                                chatId: chatId,
                                video: InputFile.FromStream(stream, fileName),
                                caption: caption,
                                parseMode: ParseMode.Markdown
                            );
                            break;

                        default:
                            await _botClient.SendDocumentAsync(
                                chatId: chatId,
                                document: InputFile.FromStream(stream, fileName),
                                caption: caption,
                                parseMode: ParseMode.Markdown
                            );
                            break;
                    }
                }
            }
            else
            {
                if(caption != null)
                    await _botClient.SendTextMessageAsync(chatId:chatId, text: caption);
            }
        }
        private static ConcurrentQueue<TelegramPostRequest> activeRequests =
            new ();
        public TelegramPostCreator(IServiceProvider services) 
        {
            _services = services;
        }
        public void AddPostToLine(TelegramPostRequest req)
        {
            activeRequests.Enqueue(req);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(async () =>
        {
            var scope = _services.CreateScope();
            _botClient =  scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<TelegramPostCreator>>();
            _cfg = scope.ServiceProvider.GetRequiredService<AppConfig>();
            while(true)
            {
                try
                {
                    if (_cfg.ASPNETCORE_ENVIRONMENT == AppConfig.LOCAL)
                        continue;
                    if (activeRequests.Count == 0)
                        continue;
                    if (!activeRequests.TryDequeue(out var request))
                        continue;
                    
                    
                    foreach (var subscriber in request.Viewers)
                    {
                        await Task.Delay(AppConfig.DELAY_PER_REQUEST);
                        try
                        {
                            await SendFileAsync(subscriber, request.Media, request.Message);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Произошла ошибка при отправке поста " +
                                $"подписчику {subscriber} для стримера {request.StreamerId}\n" +
                                $"{ex.Message}");
                            continue;
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
