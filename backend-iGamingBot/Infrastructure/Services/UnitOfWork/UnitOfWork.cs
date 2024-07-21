
using backend_iGamingBot.Infrastructure.Configs;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppCtx _ctx;

        public UnitOfWork(AppCtx ctx) => _ctx = ctx;

        public IDbTransaction BeginTransaction()
        {
           var transaction = _ctx.Database.BeginTransaction();
            return transaction.GetDbTransaction();
        }

        public void ClearCache()
        {
            _ctx.ChangeTracker.Clear();
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
