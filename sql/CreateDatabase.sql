-- TCG Tournament Manager - SQL Server Schema Script
-- Run against SQL Server Express or LocalDB

IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'TcgTournamentManager')
BEGIN
    CREATE DATABASE [TcgTournamentManager];
END
GO

USE [TcgTournamentManager];
GO

-- Note: Entity Framework migrations are the primary schema management tool.
-- This script documents the expected schema structure.

/*
Master tables (MST_*):
  - MST_TOURNAMENT
  - MST_PLAYER
  - MST_ORGANIZER_USER
  - MST_ROUND
  - MST_MATCH

Child/detail tables:
  - TOURNAMENT_PLAYER
  - MATCH_RESULT
  - TOURNAMENT_STANDING
  - BYE_HISTORY
  - TOP_CUT_BRACKET
  - AUDIT_LOG

To apply schema via EF Core:
  dotnet ef database update --project TcgTournamentManager.Infrastructure --startup-project TcgTournamentManager.Api

To backup:
  POST /api/database/backup

To restore:
  POST /api/database/restore { "backupPath": "C:\\path\\to\\backup.bak" }
*/