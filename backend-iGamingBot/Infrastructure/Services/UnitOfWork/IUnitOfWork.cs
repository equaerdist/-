namespace backend_iGamingBot.Infrastructure.Services.UnitOfWork
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
