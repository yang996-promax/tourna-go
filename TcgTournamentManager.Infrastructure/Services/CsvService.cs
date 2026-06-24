using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Infrastructure.Services;

public class CsvService : ICsvService
{
    private readonly IPlayerService _playerService;
    private readonly IPlayerRepository _playerRepo;

    public CsvService(IPlayerService playerService, IPlayerRepository playerRepo)
    {
        _playerService = playerService;
        _playerRepo = playerRepo;
    }

    public async Task<int> ImportPlayersAsync(int tournamentId, Stream csvStream, CancellationToken ct = default)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            MissingFieldFound = null,
            HeaderValidated = null
        };

        using var reader = new StreamReader(csvStream);
        using var csv = new CsvReader(reader, config);
        var rows = csv.GetRecords<PlayerCsvImportRow>().ToList();

        var count = 0;
        foreach (var row in rows)
        {
            await _playerService.AddPlayerAsync(tournamentId, new CreatePlayerRequest(
                row.PlayerId ?? row.ExternalPlayerId ?? $"P{row.PlayerNumber}",
                row.Name ?? row.PlayerName ?? "Unknown",
                row.ContactNumber,
                row.DeckName,
                row.PlayerNumber > 0 ? row.PlayerNumber : null), ct);
            count++;
        }

        return count;
    }

    public async Task<byte[]> ExportPlayersAsync(int tournamentId, CancellationToken ct = default)
    {
        var players = await _playerRepo.GetTournamentPlayersAsync(tournamentId, ct);
        var sb = new StringBuilder();
        sb.AppendLine("PlayerId,PlayerName,ContactNumber,DeckName,PlayerNumber");

        foreach (var p in players)
        {
            sb.AppendLine(string.Join(",",
                Escape(p.Player.ExternalPlayerId),
                Escape(p.Player.Name),
                Escape(p.Player.ContactNumber ?? ""),
                Escape(p.DeckName ?? ""),
                p.PlayerNumber));
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    private static string Escape(string value)
    {
        if (value.Contains(',') || value.Contains('"'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }

    private sealed class PlayerCsvImportRow
    {
        public string? PlayerId { get; set; }
        public string? ExternalPlayerId { get; set; }
        public string? Name { get; set; }
        public string? PlayerName { get; set; }
        public string? ContactNumber { get; set; }
        public string? DeckName { get; set; }
        public int PlayerNumber { get; set; }
    }
}