namespace backend_iGamingBot.Infrastructure.Services
{
    public class PollingService : PollingServiceBase<ReceiverService>
    {
        public PollingService(IServiceProvider serviceProvider, ILogger<PollingService> logger)
            : base(serviceProvider, logger)
        {
        }
    }
}
