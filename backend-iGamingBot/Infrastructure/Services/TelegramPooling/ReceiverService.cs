using Telegram.Bot.Polling;
using Telegram.Bot;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class ReceiverService : ReceiverServiceBase<UpdateHandler>
    {
        public ReceiverService(
            ITelegramBotClient botClient,
        UpdateHandler updateHandler,
            ILogger<ReceiverServiceBase<UpdateHandler>> logger)
            : base(botClient, updateHandler, logger)
        {
        }
    }
}
