
using backend_iGamingBot.Infrastructure.Configs;

namespace backend_iGamingBot.Infrastructure.Services
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppCtx _ctx;

        public UnitOfWork(AppCtx ctx) => _ctx = ctx;
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _ctx.SaveChangesAsync(cancellationToken);
        }
    }
}
