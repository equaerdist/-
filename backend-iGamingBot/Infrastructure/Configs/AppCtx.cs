using backend_iGamingBot.Models;
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
        public DbSet<Subscriber> Subscribers { get; set; } = null!;
        public DbSet<WinnerNote> WinnerNotes { get; set; } = null!;
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
                .WithMany(r => r.Participants)
                .UsingEntity<ParticipantNote>(j =>
                {
                    j.HasOne(s => s.Participant)
                    .WithMany(u => u.ParticipantNotes)
                    .HasForeignKey(s => s.ParticipantId);

                    j.HasOne(s => s.Raffle)
                    .WithMany(r => r.ParticipantsNote)
                    .HasForeignKey(n => n.RaffleId);

                    j.HasKey(s => s.Id);
                    j.Property(s => s.HaveAbused).HasDefaultValue(false);
                });
            modelBuilder.Entity<DefaultUser>()
                .HasMany(u => u.WinnerRaffles)
                .WithMany(r => r.Winners)
                .UsingEntity<WinnerNote>(j =>
                {
                    j.HasOne(n => n.Winner)
                    .WithMany(u => u.WinnerNotes)
                    .HasForeignKey(n => n.WinnerId);

                    j.HasOne(n => n.Raffle)
                    .WithMany (r => r.WinnersNote)
                    .HasForeignKey(n => n.RaffleId);

                    j.HasKey(j => new { j.WinnerId, j.RaffleId });
                    j.Property(j => j.AmountOfWins).HasDefaultValue(1);
                });
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
            modelBuilder.Entity<DefaultUser>()
                .HasMany(u => u.UserPayMethods)
                .WithOne(p => p.User);
            modelBuilder.Entity<UserPayMethod>()
                .HasKey(m => new { m.UserId, m.Platform });
        }
    }
}
