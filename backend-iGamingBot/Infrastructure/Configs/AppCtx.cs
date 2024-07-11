using Microsoft.EntityFrameworkCore;

namespace backend_iGamingBot.Infrastructure.Configs
{
    public class AppCtx : DbContext
    {
        public DbSet<Config> Configs { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Config>()
                .HasIndex(u => u.Name)
                .IsUnique();
            modelBuilder.Entity<Config>()
                .Property(u => u.Payload)
                .HasColumnType("json");
        }
    }
}
