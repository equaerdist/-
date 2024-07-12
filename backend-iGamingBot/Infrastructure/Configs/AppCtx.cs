using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace backend_iGamingBot.Infrastructure.Configs
{
    public class AppCtx : DbContext
    {
        public DbSet<Config> Configs { get; set; } = null!;
        public DbSet<Streamer> Streamers { get; set; } = null!;
        public AppCtx(DbContextOptions<AppCtx> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Config>()
                .HasIndex(u => u.Name)
                .IsUnique();
            modelBuilder.Entity<Config>()
                .Property(u => u.Payload)
                .HasColumnType("json");
            modelBuilder.Entity<Streamer>()
             .Property(e => e.Socials)
             .HasConversion(
                 v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                 v => JsonSerializer.Deserialize<List<Social>>(v, new JsonSerializerOptions())!,
                 new ValueComparer<List<Social>>(
                     (c1, c2) => c1!.SequenceEqual(c2!),
                     c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Parameter.IsLive.GetHashCode() + v.Parameter.Link == null ? 3 : 2)),
                     c => c.ToList()));
        }
    }
}
