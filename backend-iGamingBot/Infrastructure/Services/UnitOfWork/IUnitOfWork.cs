using System.Data;

namespace backend_iGamingBot.Infrastructure.Services
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync(CancellationToken cancellationToken = default);
        public IDbTransaction BeginTransaction();
    }
}
