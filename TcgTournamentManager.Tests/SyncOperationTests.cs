using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Infrastructure.Data;
using TcgTournamentManager.Infrastructure.Repositories;

namespace TcgTournamentManager.Tests;

public class SyncOperationTests : IDisposable
{
    private readonly TournamentDbContext _db;
    private readonly TournamentRepository _tournamentRepo;
    private readonly PlayerRepository _playerRepo;

    public SyncOperationTests()
    {
        var options = new DbContextOptionsBuilder<TournamentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _db = new TournamentDbContext(options);
        _tournamentRepo = TestRepositories.Tournament(_db);
        _playerRepo = TestRepositories.Player(_db);
    }

    [Fact]
    public async Task DeleteAsync_MarksTournamentSyncOperationD()
    {
        var tournament = await SeedTournamentAsync();
        await _tournamentRepo.DeleteAsync(tournament.Id);

        (await _tournamentRepo.GetByIdAsync(tournament.Id)).Should().BeNull();

        var deleted = await _db.Tournaments
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tournament.Id);

        deleted.Should().NotBeNull();
        deleted!.SyncOperation.Should().Be(SyncOperation.D);
        deleted.SyncVersion.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
        deleted.Version.Should().Be(2);
    }

    [Fact]
    public async Task RemoveFromTournamentAsync_MarksPlayerSyncOperationD()
    {
        var tournament = await SeedTournamentAsync();
        var tp = await SeedTournamentPlayerAsync(tournament.Id);

        await _playerRepo.RemoveFromTournamentAsync(tp.Id);

        var active = await _playerRepo.GetTournamentPlayersAsync(tournament.Id);
        active.Should().BeEmpty();

        var deleted = await _db.TournamentPlayers
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(p => p.Id == tp.Id);

        deleted.Should().NotBeNull();
        deleted!.SyncOperation.Should().Be(SyncOperation.D);
    }

    [Fact]
    public async Task GetDeletedTournamentPlayerAsync_FindsRemovedRegistration()
    {
        var tournament = await SeedTournamentAsync();
        var tp = await SeedTournamentPlayerAsync(tournament.Id);
        await _playerRepo.RemoveFromTournamentAsync(tp.Id);

        var found = await _playerRepo.GetDeletedTournamentPlayerAsync(tournament.Id, tp.PlayerId);
        found.Should().NotBeNull();
        found!.Id.Should().Be(tp.Id);
    }

    private async Task<Tournament> SeedTournamentAsync()
    {
        var tournament = new Tournament
        {
            Name = "Sync Test",
            GameTitle = "Pokémon TCG",
            EventDate = DateTime.UtcNow,
            Organizer = "Test",
            Venue = "Test",
            TotalSwissRounds = 3,
            Status = TournamentStatus.Registration
        };
        return await _tournamentRepo.CreateAsync(tournament);
    }

    private async Task<TournamentPlayer> SeedTournamentPlayerAsync(int tournamentId)
    {
        var player = await _playerRepo.CreatePlayerAsync(new Player { ExternalPlayerId = "SY1", Name = "Test Player" });
        return await _playerRepo.AddToTournamentAsync(new TournamentPlayer
        {
            TournamentId = tournamentId,
            PlayerId = player.Id,
            PlayerNumber = 1
        });
    }

    public void Dispose() => _db.Dispose();
}