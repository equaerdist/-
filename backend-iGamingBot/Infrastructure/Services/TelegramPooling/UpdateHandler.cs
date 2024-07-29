﻿using Telegram.Bot.Polling;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class UpdateHandler : IUpdateHandler
    {
        private ITelegramBotClient _botClient;
        private readonly ILogger<UpdateHandler> _logger;
        private readonly AppConfig _cfg;
        private readonly IUserService _userSrv;
        private readonly IUserRepository _userSrc;
        private readonly ITelegramExtensions _tgExt;

        public UpdateHandler(ITelegramBotClient botClient,
            ILogger<UpdateHandler> logger,
           IUserRepository userSrc,
           IUserService userSrv,
            AppConfig cfg,
            ITelegramExtensions tgExt)
        {
            _botClient = botClient;
            _logger = logger;
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

      

        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var message = update.Message;
            if (message is null || message.From is null || message.From.IsBot)
            {
                await botClient.SendTextMessageAsync(message!.Chat.Id, AppDictionary.TelegramUserNotDefined);
            }
            else
            {
                try
                {
                    await CheckForExisting(message, cancellationToken);
                }
                catch (AppException ex)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, ex.Message);
                    return;
                }
                catch(Exception ex)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, ex.Message);
                }
                await SendMenu(message, AppDictionary.WelcomeMessage);
            }
        }
    }
}
