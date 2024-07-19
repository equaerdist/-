namespace backend_iGamingBot.Infrastructure.Services
{
   
    public interface IReceiverService
    {
        Task ReceiveAsync(CancellationToken stoppingToken);
    }
        
}
