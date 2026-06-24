using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TcgTournamentManager.Core;
using TcgTournamentManager.Core.DTOs;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Interfaces;

namespace TcgTournamentManager.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IOrganizerUserRepository _userRepo;
    private readonly IConfiguration _configuration;

    public AuthService(IOrganizerUserRepository userRepo, IConfiguration configuration)
    {
        _userRepo = userRepo;
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByUsernameAsync(request.Username, ct: ct)
            ?? throw new UnauthorizedAccessException("Invalid credentials.");

        if (!VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return GenerateToken(user);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterOrganizerRequest request, CancellationToken ct = default)
    {
        var orgCd = string.IsNullOrWhiteSpace(request.OrgCD) ? OrgDefaults.DefaultOrgCD : request.OrgCD.Trim();

        if (await _userRepo.GetByUsernameAsync(request.Username, orgCd, ct) != null)
            throw new InvalidOperationException("Username already exists.");

        var user = await _userRepo.CreateAsync(new OrganizerUser
        {
            Username = request.Username,
            PasswordHash = HashPassword(request.Password),
            DisplayName = request.DisplayName,
            OrgCD = orgCd
        }, ct);

        return GenerateToken(user);
    }

    public async Task EnsureDefaultUserAsync(CancellationToken ct = default)
    {
        if (await _userRepo.CountAsync(ct) > 0) return;

        await _userRepo.CreateAsync(new OrganizerUser
        {
            Username = "admin",
            PasswordHash = HashPassword("admin123"),
            DisplayName = "Tournament Organizer",
            OrgCD = OrgDefaults.DefaultOrgCD
        }, ct);
    }

    private LoginResponse GenerateToken(OrganizerUser user)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var expires = DateTime.UtcNow.AddHours(double.Parse(jwtSettings["ExpireHours"] ?? "12"));

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("display_name", user.DisplayName),
            new Claim("org_cd", user.OrgCD)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: expires,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            user.Username,
            user.DisplayName,
            expires,
            user.OrgCD);
    }

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    private static bool VerifyPassword(string password, string stored)
    {
        var parts = stored.Split('.');
        if (parts.Length != 2) return false;
        var salt = Convert.FromBase64String(parts[0]);
        var hash = Convert.FromBase64String(parts[1]);
        var attempt = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(hash, attempt);
    }
}