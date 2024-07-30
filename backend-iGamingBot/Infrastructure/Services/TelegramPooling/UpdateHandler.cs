using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Collections.Concurrent;
using backend_iGamingBot.Models;


namespace backend_iGamingBot.Infrastructure.Services
{
    public class UpdateHandler : IUpdateHandler
    {
        private ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly IStreamerService _streamerSrv;
        private readonly AppConfig _cfg;
        private readonly IUserService _userSrv;
        private readonly IUserRepository _userSrc;
        private readonly ITelegramExtensions _tgExt;
        private readonly static ConcurrentDictionary<long, AdminState> _adminDialogs = new();
        private readonly static long[] _admins = [891734544, 1666815053, 192267082, 1341625052];

        public UpdateHandler(ITelegramBotClient botClient,
            ILogger<UpdateHandler> logger,
           IUserRepository userSrc,
           IUserService userSrv,
            AppConfig cfg,
            ITelegramExtensions tgExt,
            IStreamerService streamerSrv)
        {
            _botClient = botClient;
            _logger = logger;
            _streamerSrv = streamerSrv;
            _cfg = cfg;
            _userSrv = userSrv;
            _userSrc = userSrc;
            _tgExt = tgExt;
        }
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            _logger.LogError($"Error in bot ocurred {exception.Message}");
            return Task.CompletedTask;
        }
        private async Task<bool> CheckForExisting(Message msg, CancellationToken cancellationToken)
        {
            var message = msg;
            string[] startParams = message.Text!.Split(' ');
            string? streamerName = null;
            if (startParams.Length > 1)
                streamerName = startParams[1];

            if (message.From is null)
            {
                _logger.LogInformation(AppDictionary.TelegramUserNotDefined);
                return false;
            }
            long userId = message!.From.Id;
           if(streamerName is null)
           {
                try
                {
                    var userFromDb = await _userSrc.GetUserByIdAsync(userId.ToString());
                    await CheckUserInformation(msg);
                    _logger.LogInformation(AppDictionary.UserAlreadyExists);
                    return true;
                }
                catch(InvalidOperationException)
                {
                    await _userSrv.RegisterUser(new()
                    {
                        FirstName = msg.From!.FirstName,
                        LastName = msg.From.LastName,
                        TgId = userId.ToString(),
                        ImageUrl = await GetUserImageUrl(msg.From.Id),
                        Username = msg.From.Username
                    });
                    return true;
                }
            }
           else
           {
                await _userSrv.RegisterStreamer(new()
                {
                    FirstName = msg.From!.FirstName,
                    LastName = msg.From.LastName,
                    TgId = userId.ToString(),
                    Name = streamerName,
                    ImageUrl = await GetUserImageUrl(msg.From.Id),
                    Username = msg.From.Username
                });
                return true;
           }
        }
        private async Task<string?> GetUserImageUrl(long id) => await _tgExt.GetUserImageUrl(id);
        private async Task CheckUserInformation(Message msg)
        {
            await _userSrv.CheckUserInformation(new() 
            { 
                FirstName = msg.From!.FirstName, 
                LastName = msg.From!.LastName,
                TgId = msg.From.Id.ToString(),
                ImageUrl = await GetUserImageUrl(msg.From.Id),
                Username = msg.From.Username
            });
        }
        private async Task SendMenu(Message msg, string text)
        {
            var chatId = msg.Chat.Id;
            var tgId = msg.From!.Id;
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
                new []
                {
                    InlineKeyboardButton.WithWebApp(AppDictionary.OpenWebApp, 
                    new(){ Url = _cfg.Frontend})
                }
            });

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: text,
                replyMarkup: inlineKeyboard
            );
        }
        private bool IsAdminDialog(Message msg)
        {
            return _adminDialogs.Keys.Contains(msg.From!.Id) 
                || (msg.Text != null && msg.Text.StartsWith("/add_admin"));
        }
        private async Task HandleAdminDialog(Message msg)
        {
            var text = msg.Text;
            var chatId = msg.From!.Id;
            if (!string.IsNullOrEmpty(text) && text.StartsWith("/add_admin"))
            {
                var result = AppDictionary.Denied;
                if(_admins.Contains(chatId))
                    result = AppDictionary.StreamerNameRequest;
                await _botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: result
                );
                if(_admins.Contains(chatId))
                    _adminDialogs[chatId] = new AdminState() { AdminStep = AdminStep.AwaitingName };
            }
            else if(_adminDialogs.ContainsKey(chatId))
            {
                var streamerName = msg.Text;
                if (string.IsNullOrEmpty(streamerName))
                    throw new AppException(AppDictionary.BadNameRequest);
                var inviteCode = await _streamerSrv.CreateStreamerInvite(streamerName);
                await _botClient.SendTextMessageAsync(
                  chatId: chatId,
                  text: $"Ссылка для стримера {_cfg.BotName}?start={inviteCode}");
                _adminDialogs.Remove(chatId, out var state);
            }
        }

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
            CancellationToken cancellationToken)
        {
            var message = update.Message;
            var alreadyHandled = false;
            string? text = message?.Text;
            if (message is null || message.From is null || message.From.IsBot)
            {
                await botClient.SendTextMessageAsync(message!.Chat.Id, AppDictionary.TelegramUserNotDefined);
            }
            try 
            {
                if(IsAdminDialog(message))
                {
                    alreadyHandled = true;
                    await HandleAdminDialog(message);
                }
                if (!alreadyHandled)
                {
                    await CheckForExisting(message, cancellationToken);
                    await SendMenu(message, AppDictionary.WelcomeMessage);
                }
            }
            catch (AppException ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, ex.Message);
                return;
            }
            catch (Exception ex)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, ex.Message);
            }
            
        }
    }
}
