using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace backend_iGamingBot.Infrastructure.Configs
{
    public class AppCtx : DbContext
    {
        public DbSet<Config> Configs { get; set; } = null!;
        public DbSet<Streamer> Streamers { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Raffle> Raffles { get; set; } = null!;
        public DbSet<DefaultUser> AllUsers { get; set; } = null!;
        public DbSet<Subscriber> Subscribers { get; set; }
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
                     c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Parameter.GetHashCode())),
                     c => c.ToList()));
            modelBuilder.Entity<User>()
                .HasIndex(u => u.TgId)
                .IsUnique();
            modelBuilder.Entity<Streamer>()
                .HasIndex(u =>u.TgId)
                .IsUnique();
            modelBuilder.Entity<Streamer>()
                .HasIndex(s => s.Name)
                .IsUnique();
            modelBuilder.Entity<Streamer>()
                .HasMany(s => s.Subscribers)
                .WithMany(U => U.Streamers)
                .UsingEntity<Subscriber>(j =>
                {
                    j.HasOne(s => s.Streamer)
                    .WithMany(s => s.SubscribersRelation)
                    .HasForeignKey(s => s.StreamerId);

                    j.HasOne(s => s.User)
                    .WithMany(u => u.StreamersRelation)
                    .HasForeignKey(s => s.UserId);
                   
                    j.HasKey(r => new { r.UserId, r.StreamerId });
                });
            modelBuilder.Entity<DefaultUser>()
                .HasMany(u => u.ParticipantRaffles)
                .WithMany(r => r.Participants);
            modelBuilder.Entity<DefaultUser>()
                .HasMany(u => u.WinnerRaffles)
                .WithMany(r => r.Winners);
            modelBuilder.Entity<Streamer>()
                .HasMany(s => s.CreatedRaffles)
                .WithOne(r => r.Creator)
                .HasForeignKey(s => s.CreatorId);
            modelBuilder.Entity<Streamer>()
                .HasMany(s => s.Admins)
                .WithMany(u => u.Negotiable);
            modelBuilder.Entity<Raffle>()
               .Property(e => e.RaffleConditions)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                   v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())!,
                   new ValueComparer<List<string>>(
                       (c1, c2) => c1!.SequenceEqual(c2!),
                       c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                       c => c.ToList()));
            modelBuilder.Entity<User>()
              .Property(e => e.PayMethods)
              .HasConversion(
                  v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                  v => JsonSerializer.Deserialize<List<UserPayMethod>>(v, new JsonSerializerOptions())!,
                  new ValueComparer<List<UserPayMethod>>(
                      (c1, c2) => c1!.SequenceEqual(c2!),
                      c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Data == null ? 1 : v.Data.GetHashCode())),
                      c => c.ToList()));
        }
    }
}
