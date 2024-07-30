using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Collections.Concurrent;
using backend_iGamingBot.Dto;
using TwitchLib.Api.Helix;


namespace backend_iGamingBot.Infrastructure.Services
{
    public class UpdateHandler : IUpdateHandler
    {
        private ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly IServiceProvider _services;
        private readonly AppConfig _cfg;

        private readonly ITelegramExtensions _tgExt;
        private readonly static ConcurrentDictionary<long, AdminState> _adminDialogs = new();
        private readonly static long[] _admins = [891734544, 1666815053, 192267082, 1341625052];

        public UpdateHandler(ITelegramBotClient botClient,
            ILogger<UpdateHandler> logger,
           IServiceProvider services,
            AppConfig cfg,
            ITelegramExtensions tgExt)
        {
            _botClient = botClient;
            _logger = logger;
            _services = services;
            _cfg = cfg;
           
            _tgExt = tgExt;
        }
        public Task HandlePollingErrorAsync(ITelegramBotClient botClient, 
            Exception exception, 
            CancellationToken cancellationToken)
        {
            _logger.LogError($"Error in bot ocurred {exception.Message}");
            return Task.CompletedTask;
        }
        private async Task RegisterDefaultUser(Message msg, IUserService _userSrv, IUserRepository _userSrc)
        {
            var userId = msg.From!.Id;
            try
            {
                var userFromDb = await _userSrc.GetUserByIdAsync(userId.ToString());
                await CheckUserInformation(msg, _userSrv);
                _logger.LogInformation(AppDictionary.UserAlreadyExists);
            }
            catch (InvalidOperationException)
            {
                await _userSrv.RegisterUser(new()
                {
                    FirstName = msg.From!.FirstName,
                    LastName = msg.From.LastName,
                    TgId = userId.ToString(),
                    ImageUrl = await GetUserImageUrl(msg.From.Id),
                    Username = msg.From.Username
                });
            }
        }
        private static string? GetStartParam(Message msg)
        {
            var message = msg;
            string[] startParams = message.Text!.Split(' ');
            string? param = null;
            if (startParams.Length > 1)
                param = startParams[1];
            return param;
        }
        private async Task<bool> CheckForExisting(Message msg, CancellationToken cancellationToken)
        {
            using var scope = _services.CreateScope();
            var _userSrv = scope.ServiceProvider.GetRequiredService<IUserService>();
            var _userSrc = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            var message = msg;
            string? streamerName = GetStartParam(message);

            if (message.From is null)
            {
                _logger.LogInformation(AppDictionary.TelegramUserNotDefined);
                return false;
            }
            long userId = message!.From.Id;
           if(streamerName is null)
           {
                await RegisterDefaultUser(msg, _userSrv, _userSrc);
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
           }
            return true;
        }
        private async Task<string?> GetUserImageUrl(long id) => await _tgExt.GetUserImageUrl(id);
        private async Task CheckUserInformation(Message msg, IUserService _userSrv)
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
            using var scope = _services.CreateScope();
            var _streamerSrv = scope.ServiceProvider.GetRequiredService<IStreamerService>();
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
        private bool IsAdminInviteDialog(Message msg)
        {
            var param = GetStartParam(msg);
            return !string.IsNullOrEmpty(param) && param.Contains(AppDictionary.AdminInvite);
        }
        private async Task HandleAdminInviteDialog(Message msg)
        {
            using var scope = _services.CreateScope();
            var _streamerSrv = scope.ServiceProvider.GetRequiredService<IStreamerService>();
            var chatId = msg.From!.Id;
            var param = GetStartParam(msg);
            var _userSrv = scope.ServiceProvider.GetRequiredService<IUserService>();
            var _userSrc = scope.ServiceProvider.GetRequiredService<IUserRepository>();
            await RegisterDefaultUser(msg, _userSrv, _userSrc);
            var req = new AdminInviteRequest()
            { 
                Command = param!, 
                UserId = msg.From!.Id.ToString() 
            };
            await _streamerSrv.HandleAdminInvite(req);
            await _botClient.SendTextMessageAsync(
                 chatId: chatId,
                 text: AppDictionary.AdminApplied);
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
                if (IsAdminInviteDialog(message))
                {
                    alreadyHandled = true;
                    await HandleAdminInviteDialog(message);
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
