# Tourna-Go

A self-hosted, offline-capable tournament management application for local TCG organizers. Supports Swiss rounds and single-elimination top cut playoffs for Pokémon TCG, Magic: The Gathering, Yu-Gi-Oh!, One Piece, and Union Arena style events.

## Technology Stack

- **Backend:** ASP.NET Core 8 Web API, Entity Framework Core, SQL Server LocalDB
- **Frontend:** React 19, TypeScript, TailwindCSS, Vite
- **Auth:** Local JWT login

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 20+](https://nodejs.org/)
- SQL Server LocalDB (included with Visual Studio) or SQL Server Express

## Quick Start

### 1. Backend

### Local secrets (required)

Do **not** put your real SQL Server instance or JWT key in committed config files.

**Option A — `appsettings.Development.json` (recommended)**

```powershell
copy TcgTournamentManager.Api\appsettings.Development.json.example TcgTournamentManager.Api\appsettings.Development.json
```

Edit `appsettings.Development.json` with your SQL Server instance and a long random `Jwt:Key`.

**Option B — .NET user secrets**

```powershell
cd TcgTournamentManager.Api
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.\YOUR_INSTANCE;Database=TcgTournamentManager;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "your-long-random-secret"
```

**Option C — environment variables**

```powershell
$env:ConnectionStrings__DefaultConnection="Server=.\YOUR_INSTANCE;Database=TcgTournamentManager;Trusted_Connection=True;TrustServerCertificate=True"
$env:JWT__KEY="your-long-random-secret"
```

For E2E scripts, copy `scripts/_local-config.ps1.example` to `scripts/_local-config.ps1`.

```powershell
cd tourna-go
dotnet restore
dotnet ef database update --project TcgTournamentManager.Infrastructure --startup-project TcgTournamentManager.Api
dotnet run --project TcgTournamentManager.Api
```

API runs at `http://localhost:5284`. Swagger UI available in Development mode.

Default login: **admin** / **admin123**

### 2. Frontend

```powershell
cd tcg-tournament-client
npm install
npm run dev
```

UI runs at `http://localhost:5173`

## Connection String

`appsettings.json` ships with a generic LocalDB default only. Override locally using one of the options in **Local secrets** above.

Examples:

| Instance | Connection string |
|----------|-------------------|
| LocalDB | `Server=(localdb)\mssqllocaldb;Database=TcgTournamentManager;Trusted_Connection=True;TrustServerCertificate=True` |
| SQL Express | `Server=.\SQLEXPRESS;Database=TcgTournamentManager;Trusted_Connection=True;TrustServerCertificate=True` |

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | Local login |
| POST | `/api/tournaments` | Create tournament |
| GET | `/api/tournaments/dashboard` | Dashboard data |
| POST | `/api/tournaments/{id}/start` | Start tournament |
| POST | `/api/tournaments/{id}/players` | Add player |
| POST | `/api/tournaments/{id}/players/import` | Import CSV |
| GET | `/api/tournaments/{id}/players/export` | Export CSV |
| POST | `/api/tournaments/{id}/generate-round` | Generate Swiss round |
| POST | `/api/matches/{id}/result` | Enter match result |
| GET | `/api/tournaments/{id}/standings` | Get standings |
| POST | `/api/tournaments/{id}/generate-topcut` | Generate top cut |
| POST | `/api/database/backup` | Backup database |
| POST | `/api/database/restore` | Restore database |

## Swiss Pairing

See [docs/SwissPairingAlgorithm.md](docs/SwissPairingAlgorithm.md) for flowchart, pseudocode, and implementation details.

## Running Tests

```powershell
dotnet test
```

## Project Structure

```
pairing_system/
├── TcgTournamentManager.Api/          # Web API controllers
├── TcgTournamentManager.Core/       # Entities, DTOs, interfaces
├── TcgTournamentManager.Infrastructure/  # EF Core, repositories, services
├── TcgTournamentManager.Tests/      # Unit tests
├── tcg-tournament-client/           # React frontend
├── docs/                            # Algorithm documentation
└── sql/                             # SQL scripts
```

## Local Network Access

To allow other devices on your LAN:

1. Run API with `dotnet run --project TcgTournamentManager.Api --urls "http://0.0.0.0:5284"`
2. Update Vite proxy target or set `VITE_API_URL` to your machine's IP
3. Ensure Windows Firewall allows port 5284