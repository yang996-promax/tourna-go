using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Enums;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Infrastructure.Data;

public class SampleDataSeeder
{
    private readonly TournamentDbContext _db;
    private readonly ITournamentRepository _tournamentRepo;
    private readonly IPlayerRepository _playerRepo;
    private readonly IAuditLogRepository _auditRepo;

    public SampleDataSeeder(
        TournamentDbContext db,
        ITournamentRepository tournamentRepo,
        IPlayerRepository playerRepo,
        IAuditLogRepository auditRepo)
    {
        _db = db;
        _tournamentRepo = tournamentRepo;
        _playerRepo = playerRepo;
        _auditRepo = auditRepo;
    }

    public async Task SeedIfEmptyAsync(CancellationToken ct = default)
    {
        if (_db.Tournaments.Any()) return;

        var tournament = await _tournamentRepo.CreateAsync(new Tournament
        {
            Name = "Spring Regional Championship",
            GameTitle = "Pokémon TCG",
            EventDate = DateTime.UtcNow.Date.AddDays(7),
            Organizer = "Local Game Store",
            Venue = "Community Center Hall A",
            TotalSwissRounds = 5,
            TopCutSize = TopCutSize.Top8,
            FirstRoundPairingMode = FirstRoundPairingMode.Random,
            Status = TournamentStatus.Registration
        }, ct);

        var samplePlayers = new[]
        {
            ("P001", "Ash Ketchum", "555-0101", "Pikachu EX"),
            ("P002", "Misty Waterflower", "555-0102", "Starmie Control"),
            ("P003", "Brock Harrison", "555-0103", "Geodude Aggro"),
            ("P004", "Gary Oak", "555-0104", "Blastoise Mill"),
            ("P005", "Serena Vega", "555-0105", "Sylveon Combo"),
            ("P006", "Clemont Lite", "555-0106", "Luxray Lightning"),
            ("P007", "Iris Dragon", "555-0107", "Haxorus Beatdown"),
            ("P008", "Cilan Grass", "555-0108", "Serperior Control")
        };

        var number = 1;
        foreach (var (externalId, name, contact, deck) in samplePlayers)
        {
            var player = await _playerRepo.CreatePlayerAsync(new Player
            {
                ExternalPlayerId = externalId,
                Name = name,
                ContactNumber = contact
            }, ct);

            await _playerRepo.AddToTournamentAsync(new TournamentPlayer
            {
                TournamentId = tournament.Id,
                PlayerId = player.Id,
                PlayerNumber = number++,
                DeckName = deck
            }, ct);
        }

        await _auditRepo.LogAsync("Seed", "Tournament", tournament.Id, "Sample tournament and 8 players seeded", null, ct);
    }
}