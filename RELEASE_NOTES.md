# TCG Tournament Manager — Release Notes

**Project:** `pairing_system`  
**Latest update:** June 24, 2026  
**Stack:** ASP.NET Core 8 · React 19 / TypeScript / Tailwind · SQL Server

---

## Current Features

### Authentication
- Local JWT login (`admin` / `admin123` by default)
- Protected API routes; token stored in browser local storage

### Event Management (Dashboard)
- List all tournaments/events on the main dashboard
- Create events with name, game, date, organizer, venue, Swiss round count, top cut size (None / Top 4 / Top 8 / Top 16), and first-round pairing mode (Random / Seeded)
- Click an event to open its dedicated event dashboard
- **Multi-select delete:** custom checkboxes, select-all bar, bulk delete with confirmation
- **Per-event delete:** trash icon on each event row and on the event header
- Delete confirmation modal lists affected events before removal (soft delete — data retained in database)

### Event Dashboard (per event)
- Tabbed navigation: **Overview**, **Players**, **Pairings**, **Standings**, **Top Cut**
- Overview shows event status, round progress, and quick links
- **Start Tournament** confirmation modal with player list; warns if player count is below minimum (`max(2, topCutSize)`)
- **End Swiss & Start Top Cut** button when Swiss is done and a top cut is configured

### Players
- Add, edit, and remove players (removal is a soft delete; re-adding the same player restores their registration)
- Search players
- CSV import and export
- Player numbers assigned automatically

### Swiss Pairings
- Automatic Swiss round generation (random or seeded round 1)
- Pairing algorithm avoids repeat matchups where possible
- Enter match results: 2-0, 2-1, 1-2, 0-2, Draw, Bye
- Round completion tracked per round

### Standings
- Live standings with match points and tiebreakers (OMW%, GW%, OGW%)
- Sortable display

### Top Cut (Single Elimination)
- Qualified players list seeded from Swiss standings
- Generate bracket for Top 4, Top 8, or Top 16
- Visual bracket view across rounds (R16 → QF → SF → Final as applicable)
- Standard seeding (1v8, 4v5, etc. for Top 8)
- Click a player name to record the match winner; winner auto-advances to the next round
- Champion banner when the final is decided
- Tournament status flows: `Draft` → `Registration` → `InProgress` → `SwissComplete` → `TopCutInProgress` → `Completed`

### Database
- Backup and restore endpoints
- EF Core migrations; sample data seeder available
- **All insert, update, and delete operations** go through SQL Server stored procedures (`usp_*`); EF Core is used for reads only
- Stored procedures are deployed automatically on API startup (`Sql/StoredProcedures.sql`)
- **Soft delete** on tournaments and tournament player registrations (`SyncOperation = 'D'`); deleted records are hidden from all app queries but kept in the database

### Tests
- Unit tests for Swiss pairing, standings, match results, and top cut bracket building
- Integration test for recording multiple top cut results in the same round
- Soft delete tests for tournament and player removal

---

## URLs & Configuration

| Service  | URL                      |
|----------|--------------------------|
| API      | `http://localhost:5284`  |
| Frontend | `http://localhost:5173`  |

Default database connection is configured in `TcgTournamentManager.Api/appsettings.json`.

---

## Changelog

### v0.5.0 — June 24, 2026

#### Changed
- **Data writes moved to stored procedures** — repositories call `usp_*` procedures for all inserts, updates, and soft deletes
- `StoredProcedureExecutor` and `StoredProcedureDeployer` added; procedures deploy after EF migrations on startup
- `MatchResultService` and `SampleDataSeeder` updated to use repositories instead of direct `DbContext` writes
- Added `UpsertMatchResultAsync` and `UpdateRoundAsync` to `IMatchRepository`

---

### v0.4.2 — June 24, 2026

#### Changed
- `SyncVersion` is now `DateTime` — set to UTC now on every create/update/delete (A/U/D)
- `Version` remains `int`, starts at **1** on create, increments by 1 on each update/delete
- Migration: `SyncVersionAsDateTime`

---

### v0.4.1 — June 24, 2026

#### Changed
- Replaced `LastUpdatedAt` with `SyncVersion` and added `Version` column on all tables
- Migration: `SyncVersionAndVersion`

---

### v0.4.0 — June 24, 2026

#### Changed
- **Database schema regenerated** — fresh `InitialCreate` migration; database dropped and recreated
- **Removed `IsDeleted` / `DeletedAt`** — soft delete now uses `SyncOperation = 'D'`
- **All tables** now have `CreatedAt`, `LastUpdatedAt`, and `SyncOperation` (`A` = Add, `U` = Update, `D` = Delete)
- Renamed `UpdatedAt` → `LastUpdatedAt` on `Tournament` and `Standing`
- `ISyncTrackable` interface replaces `ISoftDeletable`; `SaveChanges` auto-sets timestamps and sync flags

#### Migration required (destructive — drops existing data)
```powershell
dotnet ef database drop --force --project TcgTournamentManager.Infrastructure --startup-project TcgTournamentManager.Api
dotnet ef database update --project TcgTournamentManager.Infrastructure --startup-project TcgTournamentManager.Api
```

---

### v0.3.0 — June 24, 2026

#### Changed
- **All user-facing deletes now use soft delete in the database**
  - **Delete event** (`DELETE /api/tournaments/{id}`): sets `IsDeleted = true` and `DeletedAt` on the tournament instead of removing the row and cascading child data
  - **Remove player** (`DELETE /api/tournaments/{id}/players/{tournamentPlayerId}`): soft-deletes the `TournamentPlayer` registration
- EF Core global query filters hide soft-deleted tournaments and player registrations from all normal queries
- Filtered unique indexes on `(TournamentId, PlayerId)` and `(TournamentId, PlayerNumber)` allow re-registering a previously removed player
- Re-adding a player who was soft-removed restores their existing registration row

#### Added
- `ISoftDeletable` interface and `SoftDeleteExtensions` helper
- Migration `20260624105836_AddSoftDelete`
- `SoftDeleteTests` (3 tests)

#### Migration required
```powershell
dotnet ef database update --project TcgTournamentManager.Infrastructure --startup-project TcgTournamentManager.Api
```

---

### v0.2.0 — June 24, 2026

#### Fixed
- **Top cut bracket — could not record winners after first match**  
  Recording a top cut result tried to create a new `Round` row for every match in the same bracket stage (e.g. all four quarterfinals used round number `108`). The database enforces a unique `(TournamentId, RoundNumber)` index, so the second result in the same stage failed and left the UI in a broken state.  
  **Fix:** Reuse the existing top cut round for that stage when recording further match results in `TopCutService.RecordTopCutMatchAsync`.

#### Improved
- Top cut page refreshes bracket data silently after recording a result (no full-page loading flash)
- Bracket view reloads on error to stay in sync with the server
- Client-side fallback for `isPlayable` when determining clickable matches

#### Added
- `RELEASE_NOTES.md` (this file)
- `TopCutEnterResultTests.EnterTopCutResult_AllowsMultipleMatchesInSameRound` regression test

---

### v0.1.0 — June 24, 2026 (initial build)

#### Added — Core application
- Full-stack TCG Tournament Manager moved to `D:\Archive\pairing_system`
- ASP.NET Core 8 Web API with layered architecture (Api / Core / Infrastructure / Tests)
- React + TypeScript + Tailwind frontend (`tcg-tournament-client`)
- SQL Server persistence with EF Core migrations
- Swiss pairing service with documented algorithm (`docs/SwissPairingAlgorithm.md`)
- Standings calculation with tiebreakers
- Top cut bracket generation, seeding, and winner advancement
- Local JWT authentication
- CSV player import/export
- Database backup/restore API

#### Added — Frontend navigation restructure
- Main dashboard lists events; each event opens its own dashboard
- Event layout with tabs: Overview, Players, Pairings, Standings, Top Cut
- `EventContext` for shared event state across tabs

#### Added — Top cut UI
- `TopCutPage` with qualified players list and generate-bracket action
- `TopCutBracketView` visual bracket with round columns
- Champion display when the final is complete
- `POST /api/tournaments/{id}/complete-swiss` — End Swiss & Start Top Cut flow
- `POST /api/tournaments/{id}/generate-topcut` — generate bracket
- `GET /api/tournaments/{id}/topcut` — bracket tree
- `POST /api/matches/topcut/{bracketId}/result` — record winner

#### Added — Start tournament UX
- `StartTournamentModal` with player list preview
- Backend validation for minimum player count before start

#### Added — Event deletion
- `DELETE /api/tournaments/{id}` API endpoint
- `DeleteEventModal` confirmation component
- Dashboard multi-select with custom `Checkbox` component and select-all bar
- Per-row and per-event-header delete buttons
- `tournamentApi.delete(id)` client method

#### Fixed (earlier session)
- SQL Server LocalDB not available → documented alternate server connection
- EF cascade path error on `Match.TopCutBracketId` → `DeleteBehavior.NoAction`
- Top cut bracket two-pass save for `NextBracketId` linking
- Position-based winner advancement (odd match → Player A slot, even → Player B slot)

---

## API Reference (summary)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Login |
| GET | `/api/tournaments` | List tournaments |
| GET | `/api/tournaments/dashboard` | Dashboard summary |
| POST | `/api/tournaments` | Create tournament |
| DELETE | `/api/tournaments/{id}` | Delete tournament |
| GET | `/api/tournaments/{id}` | Get tournament |
| POST | `/api/tournaments/{id}/start` | Start tournament |
| POST | `/api/tournaments/{id}/complete-swiss` | Mark Swiss complete |
| POST | `/api/tournaments/{id}/generate-round` | Generate Swiss round |
| POST | `/api/tournaments/{id}/generate-topcut` | Generate top cut bracket |
| GET | `/api/tournaments/{id}/topcut` | Get top cut bracket tree |
| GET | `/api/tournaments/{id}/standings` | Standings |
| POST | `/api/tournaments/{id}/players` | Add player |
| POST | `/api/tournaments/{id}/players/import` | Import CSV |
| GET | `/api/tournaments/{id}/players/export` | Export CSV |
| POST | `/api/matches/{id}/result` | Enter Swiss match result |
| POST | `/api/matches/topcut/{bracketId}/result` | Enter top cut result |
| POST | `/api/database/backup` | Backup database |
| POST | `/api/database/restore` | Restore database |

---

## Running Tests

```powershell
cd D:\Archive\pairing_system
dotnet test
```

---

## Known Limitations

- Top cut match results are recorded as 2-0 wins in match history (no game-count entry for individual top cut games)
- Requires SQL Server instance configured in `appsettings.json`
- Offline/LAN use requires manual firewall and URL configuration (see `README.md`)