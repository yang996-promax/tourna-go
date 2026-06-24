using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;
using TcgTournamentManager.Infrastructure.Repositories;
using TcgTournamentManager.Infrastructure.Services;

namespace TcgTournamentManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TournamentDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<StoredProcedureExecutor>();
        services.AddScoped<StoredProcedureDeployer>();

        services.AddScoped<ITournamentRepository, TournamentRepository>();
        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IMatchRepository, MatchRepository>();
        services.AddScoped<IStandingRepository, StandingRepository>();
        services.AddScoped<IByeHistoryRepository, ByeHistoryRepository>();
        services.AddScoped<ITopCutRepository, TopCutRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<IOrganizerUserRepository, OrganizerUserRepository>();

        services.AddScoped<ISwissPairingService, SwissPairingService>();
        services.AddScoped<IStandingsService, StandingsService>();
        services.AddScoped<IMatchResultService, MatchResultService>();
        services.AddScoped<ITopCutService, TopCutService>();
        services.AddScoped<ICsvService, CsvService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDatabaseService, DatabaseService>();
        services.AddScoped<ITournamentService, TournamentService>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<SampleDataSeeder>();

        return services;
    }
}