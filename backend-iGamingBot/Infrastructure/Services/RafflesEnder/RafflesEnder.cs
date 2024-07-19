
namespace backend_iGamingBot.Infrastructure.Services.RafflesEnder
{
    public class RafflesEnder : BackgroundService
    {
        private ILogger<RafflesEnder> logger = null!;
        private IServiceProvider _services;
        private IRaffleService raffleSrv = null!;
        private IRaffleRepository raffleRepository = null!;

        public RafflesEnder(IServiceProvider services) 
        {
            _services = services;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        => Task.Run(async () => 
        { 
            using var scope = _services.CreateScope();
            logger = scope.ServiceProvider.GetRequiredService<ILogger<RafflesEnder>>();
            raffleSrv = scope.ServiceProvider.GetRequiredService<IRaffleService>();
            raffleRepository = scope.ServiceProvider.GetRequiredService<IRaffleRepository>();
            while(true)
            {
                await Task.Delay(10 * 1000);
                logger.LogInformation("Начал процедуру поиска завершенных розыгрышей");
                var endedRaffles = await raffleRepository.GetRafflesAlreadyEnded();
                if (endedRaffles.Length == 0)
                    continue;
                logger.LogInformation($"Найдено {endedRaffles.Length} завершенных розыгрышей");
                foreach (var raffle in endedRaffles)
                {
                    try
                    {
                        await raffleSrv.GenerateWinnersForRaffle(raffle, false);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError($"Не удалось сгенерировать победителей для {raffle}\n {ex.Message}");
                    }
                }
            }
        }, stoppingToken);
    }
}
