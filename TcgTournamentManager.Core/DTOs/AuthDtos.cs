namespace TcgTournamentManager.Core.DTOs;

public record LoginRequest(string Username, string Password);

public record LoginResponse(string Token, string Username, string DisplayName, DateTime ExpiresAt, string OrgCD);

public record RegisterOrganizerRequest(string Username, string Password, string DisplayName, string? OrgCD = null);