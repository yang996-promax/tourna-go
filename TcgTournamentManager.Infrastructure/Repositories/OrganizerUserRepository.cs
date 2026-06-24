using Microsoft.EntityFrameworkCore;
using TcgTournamentManager.Core.Entities;
using TcgTournamentManager.Core.Interfaces;
using TcgTournamentManager.Infrastructure.Data;

namespace TcgTournamentManager.Infrastructure.Repositories;

public class OrganizerUserRepository : IOrganizerUserRepository
{
    private readonly TournamentDbContext _db;
    private readonly StoredProcedureExecutor _sp;

    public OrganizerUserRepository(TournamentDbContext db, StoredProcedureExecutor sp)
    {
        _db = db;
        _sp = sp;
    }

    public async Task<OrganizerUser?> GetByUsernameAsync(string username, CancellationToken ct = default) =>
        await _db.OrganizerUsers.FirstOrDefaultAsync(u => u.Username == username, ct);

    public async Task<OrganizerUser> CreateAsync(OrganizerUser user, CancellationToken ct = default)
    {
        var id = await _sp.ExecuteWithIntOutputAsync("usp_OrganizerUser_Insert", "@Id", ct,
            StoredProcedureExecutor.Str("@Username", user.Username, 100),
            StoredProcedureExecutor.Str("@PasswordHash", user.PasswordHash, -1),
            StoredProcedureExecutor.Str("@DisplayName", user.DisplayName, 200),
            StoredProcedureExecutor.Bit("@IsActive", user.IsActive),
            StoredProcedureExecutor.OutInt("@Id"));

        user.Id = id;
        return (await _db.OrganizerUsers.FirstOrDefaultAsync(u => u.Id == id, ct)) ?? user;
    }

    public async Task<int> CountAsync(CancellationToken ct = default) =>
        await _db.OrganizerUsers.CountAsync(ct);
}