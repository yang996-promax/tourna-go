using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TcgTournamentManager.Core;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;

namespace TcgTournamentManager.Infrastructure.Data;

public class TournamentDbContext : DbContext
{
    public TournamentDbContext(DbContextOptions<TournamentDbContext> options) : base(options) { }

    public DbSet<Tournament> Tournaments => Set<Tournament>();
    public DbSet<Player> Players => Set<Player>();
    public DbSet<TournamentPlayer> TournamentPlayers => Set<TournamentPlayer>();
    public DbSet<Round> Rounds => Set<Round>();
    public DbSet<Match> Matches => Set<Match>();
    public DbSet<MatchResult> MatchResults => Set<MatchResult>();
    public DbSet<Standing> Standings => Set<Standing>();
    public DbSet<ByeHistory> ByeHistories => Set<ByeHistory>();
    public DbSet<TopCutBracket> TopCutBrackets => Set<TopCutBracket>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<OrganizerUser> OrganizerUsers => Set<OrganizerUser>();

    public override int SaveChanges()
    {
        ApplySyncTracking();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySyncTracking();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplySyncTracking()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<ISyncTrackable>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.SyncVersion = now;
                entry.Entity.Version = 1;
                if (entry.Entity.SyncOperation == default)
                    entry.Entity.SyncOperation = SyncOperation.A;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.SyncVersion = now;
                entry.Entity.Version++;
                if (entry.Entity.SyncOperation != SyncOperation.D)
                    entry.Entity.SyncOperation = SyncOperation.U;
            }
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureSyncTrackable(modelBuilder.Entity<Tournament>());
        ConfigureOrgScoped(modelBuilder.Entity<Tournament>());
        modelBuilder.Entity<Tournament>(e =>
        {
            e.ToTable(DbTableNames.Tournament);
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.GameTitle).HasMaxLength(100).IsRequired();
            e.Property(x => x.Organizer).HasMaxLength(200).IsRequired();
            e.Property(x => x.Venue).HasMaxLength(300).IsRequired();
            e.Property(x => x.MatchFormat).HasDefaultValue(MatchFormat.BO3);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.EventDate);
            e.HasQueryFilter(x => x.SyncOperation != SyncOperation.D);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<Player>());
        ConfigureOrgScoped(modelBuilder.Entity<Player>());
        modelBuilder.Entity<Player>(e =>
        {
            e.ToTable(DbTableNames.Player);
            e.HasKey(x => x.Id);
            e.Property(x => x.ExternalPlayerId).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasMaxLength(200).IsRequired();
            e.Property(x => x.ContactNumber).HasMaxLength(50);
            e.HasIndex(x => new { x.OrgCD, x.ExternalPlayerId }).IsUnique();
            e.HasIndex(x => x.Name);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<TournamentPlayer>());
        ConfigureOrgScoped(modelBuilder.Entity<TournamentPlayer>());
        modelBuilder.Entity<TournamentPlayer>(e =>
        {
            e.ToTable(DbTableNames.TournamentPlayer);
            e.HasKey(x => x.Id);
            e.Property(x => x.DeckName).HasMaxLength(200);
            e.HasIndex(x => new { x.TournamentId, x.PlayerNumber })
                .IsUnique()
                .HasFilter("[SyncOperation] <> 'D'");
            e.HasIndex(x => new { x.TournamentId, x.PlayerId })
                .IsUnique()
                .HasFilter("[SyncOperation] <> 'D'");
            e.HasQueryFilter(x => x.SyncOperation != SyncOperation.D);
            e.HasOne(x => x.Tournament).WithMany(t => t.TournamentPlayers).HasForeignKey(x => x.TournamentId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.Player).WithMany(p => p.TournamentPlayers).HasForeignKey(x => x.PlayerId).OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<Round>());
        ConfigureOrgScoped(modelBuilder.Entity<Round>());
        modelBuilder.Entity<Round>(e =>
        {
            e.ToTable(DbTableNames.Round);
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.TournamentId, x.RoundNumber, x.RoundType }).IsUnique();
            e.HasOne(x => x.Tournament).WithMany(t => t.Rounds).HasForeignKey(x => x.TournamentId).OnDelete(DeleteBehavior.Cascade);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<Match>());
        ConfigureOrgScoped(modelBuilder.Entity<Match>());
        modelBuilder.Entity<Match>(e =>
        {
            e.ToTable(DbTableNames.Match);
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.RoundId, x.TableNumber }).IsUnique();
            e.HasOne(x => x.Round).WithMany(r => r.Matches).HasForeignKey(x => x.RoundId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.PlayerA).WithMany().HasForeignKey(x => x.PlayerAId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.PlayerB).WithMany().HasForeignKey(x => x.PlayerBId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Winner).WithMany().HasForeignKey(x => x.WinnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.TopCutBracket).WithOne(b => b.Match).HasForeignKey<Match>(x => x.TopCutBracketId).OnDelete(DeleteBehavior.NoAction);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<MatchResult>());
        ConfigureOrgScoped(modelBuilder.Entity<MatchResult>());
        modelBuilder.Entity<MatchResult>(e =>
        {
            e.ToTable(DbTableNames.MatchResult);
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.MatchId).IsUnique();
            e.HasOne(x => x.Match).WithOne(m => m.Result).HasForeignKey<MatchResult>(x => x.MatchId).OnDelete(DeleteBehavior.Cascade);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<Standing>());
        ConfigureOrgScoped(modelBuilder.Entity<Standing>());
        modelBuilder.Entity<Standing>(e =>
        {
            e.ToTable(DbTableNames.Standing);
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.TournamentId, x.TournamentPlayerId }).IsUnique();
            e.HasIndex(x => new { x.TournamentId, x.Rank });
            e.Property(x => x.OMWPercent).HasPrecision(8, 4);
            e.Property(x => x.GWPercent).HasPrecision(8, 4);
            e.Property(x => x.OGWPercent).HasPrecision(8, 4);
            e.HasOne(x => x.Tournament).WithMany(t => t.Standings).HasForeignKey(x => x.TournamentId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.TournamentPlayer).WithMany().HasForeignKey(x => x.TournamentPlayerId).OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<ByeHistory>());
        ConfigureOrgScoped(modelBuilder.Entity<ByeHistory>());
        modelBuilder.Entity<ByeHistory>(e =>
        {
            e.ToTable(DbTableNames.ByeHistory);
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.TournamentId, x.TournamentPlayerId, x.RoundNumber }).IsUnique();
            e.HasOne(x => x.Tournament).WithMany().HasForeignKey(x => x.TournamentId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.TournamentPlayer).WithMany(tp => tp.ByeHistories).HasForeignKey(x => x.TournamentPlayerId).OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<TopCutBracket>());
        ConfigureOrgScoped(modelBuilder.Entity<TopCutBracket>());
        modelBuilder.Entity<TopCutBracket>(e =>
        {
            e.ToTable(DbTableNames.TopCutBracket);
            e.HasKey(x => x.Id);
            e.HasIndex(x => new { x.TournamentId, x.Round, x.MatchPosition }).IsUnique();
            e.HasOne(x => x.Tournament).WithMany(t => t.TopCutBrackets).HasForeignKey(x => x.TournamentId).OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.PlayerA).WithMany().HasForeignKey(x => x.PlayerAId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.PlayerB).WithMany().HasForeignKey(x => x.PlayerBId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.Winner).WithMany().HasForeignKey(x => x.WinnerId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(x => x.NextBracket).WithMany().HasForeignKey(x => x.NextBracketId).OnDelete(DeleteBehavior.Restrict);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<AuditLog>());
        ConfigureOrgScoped(modelBuilder.Entity<AuditLog>());
        modelBuilder.Entity<AuditLog>(e =>
        {
            e.ToTable(DbTableNames.AuditLog);
            e.HasKey(x => x.Id);
            e.Property(x => x.Action).HasMaxLength(100).IsRequired();
            e.Property(x => x.EntityType).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.CreatedAt);
        });

        ConfigureSyncTrackable(modelBuilder.Entity<OrganizerUser>());
        ConfigureOrgScoped(modelBuilder.Entity<OrganizerUser>());
        modelBuilder.Entity<OrganizerUser>(e =>
        {
            e.ToTable(DbTableNames.OrganizerUser);
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(100).IsRequired();
            e.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            e.HasIndex(x => new { x.OrgCD, x.Username }).IsUnique();
        });
    }

    private static void ConfigureSyncTrackable<T>(EntityTypeBuilder<T> builder) where T : class, ISyncTrackable
    {
        builder.Property(x => x.SyncOperation)
            .HasConversion<string>()
            .HasMaxLength(1)
            .IsUnicode(false)
            .IsRequired();
        builder.Property(x => x.CreatedAt).IsRequired();
        builder.Property(x => x.SyncVersion).IsRequired();
        builder.Property(x => x.Version).IsRequired();
    }

    private static void ConfigureOrgScoped<T>(EntityTypeBuilder<T> builder) where T : class, IOrgScoped
    {
        builder.Property(x => x.OrgCD)
            .HasMaxLength(OrgDefaults.OrgCDMaxLength)
            .IsRequired();
        builder.HasIndex(x => x.OrgCD);
    }
}