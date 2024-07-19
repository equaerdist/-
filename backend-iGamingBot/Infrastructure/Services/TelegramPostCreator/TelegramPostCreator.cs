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
        private IStreamerRepository _streamerSrc = null!;
        private ILogger<TelegramPostCreator> _logger = null!;
        private readonly IServiceProvider _services;

        public async Task SendFileAsync(long chatId, IFormFile file, string? caption = null)
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
        private static ConcurrentQueue<(CreatePostRequest body, string streamerId)> activeRequests =
            new ();
        public TelegramPostCreator(IServiceProvider services) 
        {
            _services = services;
        }
        public void AddPostToLine((CreatePostRequest body, string streamerId) req)
        {
            activeRequests.Append(req);
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(async () =>
        {
            var scope = _services.CreateScope();
            _botClient =  scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();
            _streamerSrc = scope.ServiceProvider.GetRequiredService<IStreamerRepository>();
            _logger = scope.ServiceProvider.GetRequiredService<ILogger<TelegramPostCreator>>();
            while(true)
            {
                try
                {
                    if (activeRequests.Count == 0)
                        continue;
                    if (!activeRequests.TryDequeue(out var request))
                        continue;
                    int batchNum = 1;
                    while (true)
                    {
                        var batch = await _streamerSrc.GetBatchOfStreamerSubscribersAsync(request.streamerId,
                            AppConfig.USER_BATCH_SIZE,
                            batchNum);
                        foreach (var subscriber in batch)
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
                            batchNum++;
                            await Task.Delay(AppConfig.DELAY_PER_REQUEST);
                        }
                        if (batch.Length < AppConfig.USER_BATCH_SIZE)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Произошла ошибка в расыльшике постов, начинаем заново\n{ex.Message}");
                    continue;
                }
            }
        });
    }
}
