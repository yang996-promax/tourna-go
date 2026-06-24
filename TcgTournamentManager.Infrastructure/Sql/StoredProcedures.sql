-- TCG Tournament Manager - data modification stored procedures
-- Sync metadata: INSERT sets CreatedAt/SyncVersion/Version=1/SyncOperation='A'
-- UPDATE sets SyncVersion, Version=Version+1, SyncOperation='U'
-- SOFT DELETE sets SyncVersion, Version=Version+1, SyncOperation='D'

-- Tournament
IF OBJECT_ID('dbo.usp_Tournament_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Tournament_Insert;
GO
CREATE PROCEDURE dbo.usp_Tournament_Insert
    @Name NVARCHAR(200),
    @GameTitle NVARCHAR(100),
    @EventDate DATETIME2,
    @Organizer NVARCHAR(200),
    @Venue NVARCHAR(300),
    @TotalSwissRounds INT,
    @TopCutSize INT,
    @FirstRoundPairingMode INT,
    @MatchFormat INT,
    @HasElimination BIT,
    @EliminationLossCount INT = NULL,
    @Status INT,
    @CurrentRound INT,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO MST_TOURNAMENT (
        OrgCD, Name, GameTitle, EventDate, Organizer, Venue,
        TotalSwissRounds, TopCutSize, FirstRoundPairingMode,
        MatchFormat, HasElimination, EliminationLossCount,
        Status, CurrentRound,
        CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (
        @OrgCD, @Name, @GameTitle, @EventDate, @Organizer, @Venue,
        @TotalSwissRounds, @TopCutSize, @FirstRoundPairingMode,
        @MatchFormat, @HasElimination, @EliminationLossCount,
        @Status, @CurrentRound,
        @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_Tournament_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Tournament_Update;
GO
CREATE PROCEDURE dbo.usp_Tournament_Update
    @Id INT,
    @Name NVARCHAR(200),
    @GameTitle NVARCHAR(100),
    @EventDate DATETIME2,
    @Organizer NVARCHAR(200),
    @Venue NVARCHAR(300),
    @TotalSwissRounds INT,
    @TopCutSize INT,
    @FirstRoundPairingMode INT,
    @MatchFormat INT,
    @HasElimination BIT,
    @EliminationLossCount INT = NULL,
    @Status INT,
    @CurrentRound INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE MST_TOURNAMENT SET
        Name = @Name,
        GameTitle = @GameTitle,
        EventDate = @EventDate,
        Organizer = @Organizer,
        Venue = @Venue,
        TotalSwissRounds = @TotalSwissRounds,
        TopCutSize = @TopCutSize,
        FirstRoundPairingMode = @FirstRoundPairingMode,
        MatchFormat = @MatchFormat,
        HasElimination = @HasElimination,
        EliminationLossCount = @EliminationLossCount,
        Status = @Status,
        CurrentRound = @CurrentRound,
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'U'
    WHERE Id = @Id;
END
GO

IF OBJECT_ID('dbo.usp_Tournament_SoftDelete', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Tournament_SoftDelete;
GO
CREATE PROCEDURE dbo.usp_Tournament_SoftDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE MST_TOURNAMENT SET
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'D'
    WHERE Id = @Id AND SyncOperation <> 'D';
END
GO

-- Player
IF OBJECT_ID('dbo.usp_Player_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Player_Insert;
GO
CREATE PROCEDURE dbo.usp_Player_Insert
    @ExternalPlayerId NVARCHAR(50),
    @Name NVARCHAR(200),
    @ContactNumber NVARCHAR(50) = NULL,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO MST_PLAYER (OrgCD, ExternalPlayerId, Name, ContactNumber, CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (@OrgCD, @ExternalPlayerId, @Name, @ContactNumber, @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_Player_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Player_Update;
GO
CREATE PROCEDURE dbo.usp_Player_Update
    @Id INT,
    @ExternalPlayerId NVARCHAR(50),
    @Name NVARCHAR(200),
    @ContactNumber NVARCHAR(50) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE MST_PLAYER SET
        ExternalPlayerId = @ExternalPlayerId,
        Name = @Name,
        ContactNumber = @ContactNumber,
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'U'
    WHERE Id = @Id;
END
GO

-- TournamentPlayer
IF OBJECT_ID('dbo.usp_TournamentPlayer_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_TournamentPlayer_Insert;
GO
CREATE PROCEDURE dbo.usp_TournamentPlayer_Insert
    @TournamentId INT,
    @PlayerId INT,
    @PlayerNumber INT,
    @DeckName NVARCHAR(200) = NULL,
    @IsDropped BIT,
    @RegisteredAt DATETIME2,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO TOURNAMENT_PLAYER (
        OrgCD, TournamentId, PlayerId, PlayerNumber, DeckName, IsDropped, RegisteredAt,
        CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (
        @OrgCD, @TournamentId, @PlayerId, @PlayerNumber, @DeckName, @IsDropped, @RegisteredAt,
        @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_TournamentPlayer_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_TournamentPlayer_Update;
GO
CREATE PROCEDURE dbo.usp_TournamentPlayer_Update
    @Id INT,
    @PlayerNumber INT,
    @DeckName NVARCHAR(200) = NULL,
    @IsDropped BIT,
    @RegisteredAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE TOURNAMENT_PLAYER SET
        PlayerNumber = @PlayerNumber,
        DeckName = @DeckName,
        IsDropped = @IsDropped,
        RegisteredAt = @RegisteredAt,
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'U'
    WHERE Id = @Id;
END
GO

IF OBJECT_ID('dbo.usp_TournamentPlayer_SoftDelete', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_TournamentPlayer_SoftDelete;
GO
CREATE PROCEDURE dbo.usp_TournamentPlayer_SoftDelete
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE TOURNAMENT_PLAYER SET
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'D'
    WHERE Id = @Id AND SyncOperation <> 'D';
END
GO

-- Round
IF OBJECT_ID('dbo.usp_Round_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Round_Insert;
GO
CREATE PROCEDURE dbo.usp_Round_Insert
    @TournamentId INT,
    @RoundNumber INT,
    @RoundType INT,
    @IsComplete BIT,
    @CompletedAt DATETIME2 = NULL,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO MST_ROUND (OrgCD, TournamentId, RoundNumber, RoundType, IsComplete, CompletedAt, CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (@OrgCD, @TournamentId, @RoundNumber, @RoundType, @IsComplete, @CompletedAt, @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_Round_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Round_Update;
GO
CREATE PROCEDURE dbo.usp_Round_Update
    @Id INT,
    @IsComplete BIT,
    @CompletedAt DATETIME2 = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE MST_ROUND SET
        IsComplete = @IsComplete,
        CompletedAt = @CompletedAt,
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'U'
    WHERE Id = @Id;
END
GO

-- Match
IF OBJECT_ID('dbo.usp_Match_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Match_Insert;
GO
CREATE PROCEDURE dbo.usp_Match_Insert
    @RoundId INT,
    @TableNumber INT,
    @PlayerAId INT = NULL,
    @PlayerBId INT = NULL,
    @IsBye BIT,
    @IsComplete BIT,
    @WinnerId INT = NULL,
    @TopCutBracketId INT = NULL,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO MST_MATCH (
        OrgCD, RoundId, TableNumber, PlayerAId, PlayerBId, IsBye, IsComplete, WinnerId, TopCutBracketId,
        CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (
        @OrgCD, @RoundId, @TableNumber, @PlayerAId, @PlayerBId, @IsBye, @IsComplete, @WinnerId, @TopCutBracketId,
        @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_Match_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Match_Update;
GO
CREATE PROCEDURE dbo.usp_Match_Update
    @Id INT,
    @TableNumber INT,
    @PlayerAId INT = NULL,
    @PlayerBId INT = NULL,
    @IsBye BIT,
    @IsComplete BIT,
    @WinnerId INT = NULL,
    @TopCutBracketId INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE MST_MATCH SET
        TableNumber = @TableNumber,
        PlayerAId = @PlayerAId,
        PlayerBId = @PlayerBId,
        IsBye = @IsBye,
        IsComplete = @IsComplete,
        WinnerId = @WinnerId,
        TopCutBracketId = @TopCutBracketId,
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'U'
    WHERE Id = @Id;
END
GO

-- MatchResult
IF OBJECT_ID('dbo.usp_MatchResult_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_MatchResult_Insert;
GO
CREATE PROCEDURE dbo.usp_MatchResult_Insert
    @MatchId INT,
    @ResultType INT,
    @PlayerAGameWins INT,
    @PlayerBGameWins INT,
    @PlayerAMatchPoints INT,
    @PlayerBMatchPoints INT,
    @RecordedAt DATETIME2,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO MATCH_RESULT (
        OrgCD, MatchId, ResultType, PlayerAGameWins, PlayerBGameWins, PlayerAMatchPoints, PlayerBMatchPoints, RecordedAt,
        CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (
        @OrgCD, @MatchId, @ResultType, @PlayerAGameWins, @PlayerBGameWins, @PlayerAMatchPoints, @PlayerBMatchPoints, @RecordedAt,
        @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_MatchResult_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_MatchResult_Update;
GO
CREATE PROCEDURE dbo.usp_MatchResult_Update
    @Id INT,
    @ResultType INT,
    @PlayerAGameWins INT,
    @PlayerBGameWins INT,
    @PlayerAMatchPoints INT,
    @PlayerBMatchPoints INT,
    @RecordedAt DATETIME2
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE MATCH_RESULT SET
        ResultType = @ResultType,
        PlayerAGameWins = @PlayerAGameWins,
        PlayerBGameWins = @PlayerBGameWins,
        PlayerAMatchPoints = @PlayerAMatchPoints,
        PlayerBMatchPoints = @PlayerBMatchPoints,
        RecordedAt = @RecordedAt,
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'U'
    WHERE Id = @Id;
END
GO

-- Standing
IF OBJECT_ID('dbo.usp_Standing_DeleteByTournament', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Standing_DeleteByTournament;
GO
CREATE PROCEDURE dbo.usp_Standing_DeleteByTournament
    @TournamentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM TOURNAMENT_STANDING WHERE TournamentId = @TournamentId;
END
GO

IF OBJECT_ID('dbo.usp_Standing_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_Standing_Insert;
GO
CREATE PROCEDURE dbo.usp_Standing_Insert
    @TournamentId INT,
    @TournamentPlayerId INT,
    @Rank INT,
    @MatchPoints INT,
    @GameWins INT,
    @GameLosses INT,
    @OMWPercent DECIMAL(8,4),
    @GWPercent DECIMAL(8,4),
    @OGWPercent DECIMAL(8,4),
    @MatchesPlayed INT,
    @MatchesWon INT,
    @MatchesLost INT,
    @MatchesDrawn INT,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO TOURNAMENT_STANDING (
        OrgCD, TournamentId, TournamentPlayerId, Rank, MatchPoints, GameWins, GameLosses,
        OMWPercent, GWPercent, OGWPercent, MatchesPlayed, MatchesWon, MatchesLost, MatchesDrawn,
        CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (
        @OrgCD, @TournamentId, @TournamentPlayerId, @Rank, @MatchPoints, @GameWins, @GameLosses,
        @OMWPercent, @GWPercent, @OGWPercent, @MatchesPlayed, @MatchesWon, @MatchesLost, @MatchesDrawn,
        @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

-- ByeHistory
IF OBJECT_ID('dbo.usp_ByeHistory_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_ByeHistory_Insert;
GO
CREATE PROCEDURE dbo.usp_ByeHistory_Insert
    @TournamentId INT,
    @TournamentPlayerId INT,
    @RoundNumber INT,
    @AssignedAt DATETIME2,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO BYE_HISTORY (OrgCD, TournamentId, TournamentPlayerId, RoundNumber, AssignedAt, CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (@OrgCD, @TournamentId, @TournamentPlayerId, @RoundNumber, @AssignedAt, @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

-- TopCutBracket
IF OBJECT_ID('dbo.usp_TopCutBracket_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_TopCutBracket_Insert;
GO
CREATE PROCEDURE dbo.usp_TopCutBracket_Insert
    @TournamentId INT,
    @Round INT,
    @MatchPosition INT,
    @PlayerAId INT = NULL,
    @PlayerBId INT = NULL,
    @WinnerId INT = NULL,
    @NextBracketId INT = NULL,
    @IsComplete BIT,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO TOP_CUT_BRACKET (
        OrgCD, TournamentId, Round, MatchPosition, PlayerAId, PlayerBId, WinnerId, NextBracketId, IsComplete,
        CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (
        @OrgCD, @TournamentId, @Round, @MatchPosition, @PlayerAId, @PlayerBId, @WinnerId, @NextBracketId, @IsComplete,
        @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

IF OBJECT_ID('dbo.usp_TopCutBracket_Update', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_TopCutBracket_Update;
GO
CREATE PROCEDURE dbo.usp_TopCutBracket_Update
    @Id INT,
    @PlayerAId INT = NULL,
    @PlayerBId INT = NULL,
    @WinnerId INT = NULL,
    @NextBracketId INT = NULL,
    @IsComplete BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE TOP_CUT_BRACKET SET
        PlayerAId = @PlayerAId,
        PlayerBId = @PlayerBId,
        WinnerId = @WinnerId,
        NextBracketId = @NextBracketId,
        IsComplete = @IsComplete,
        SyncVersion = SYSUTCDATETIME(),
        Version = Version + 1,
        SyncOperation = 'U'
    WHERE Id = @Id;
END
GO

IF OBJECT_ID('dbo.usp_TopCutBracket_DeleteByTournament', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_TopCutBracket_DeleteByTournament;
GO
CREATE PROCEDURE dbo.usp_TopCutBracket_DeleteByTournament
    @TournamentId INT
AS
BEGIN
    SET NOCOUNT ON;
    DELETE FROM TOP_CUT_BRACKET WHERE TournamentId = @TournamentId;
END
GO

-- AuditLog
IF OBJECT_ID('dbo.usp_AuditLog_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_AuditLog_Insert;
GO
CREATE PROCEDURE dbo.usp_AuditLog_Insert
    @Action NVARCHAR(100),
    @EntityType NVARCHAR(100),
    @EntityId INT = NULL,
    @Details NVARCHAR(MAX) = NULL,
    @PerformedBy NVARCHAR(MAX) = NULL,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO AUDIT_LOG (OrgCD, Action, EntityType, EntityId, Details, PerformedBy, CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (@OrgCD, @Action, @EntityType, @EntityId, @Details, @PerformedBy, @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO

-- OrganizerUser
IF OBJECT_ID('dbo.usp_OrganizerUser_Insert', 'P') IS NOT NULL DROP PROCEDURE dbo.usp_OrganizerUser_Insert;
GO
CREATE PROCEDURE dbo.usp_OrganizerUser_Insert
    @Username NVARCHAR(100),
    @PasswordHash NVARCHAR(MAX),
    @DisplayName NVARCHAR(200),
    @IsActive BIT,
    @OrgCD NVARCHAR(50),
    @Id INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Now DATETIME2 = SYSUTCDATETIME();

    INSERT INTO MST_ORGANIZER_USER (OrgCD, Username, PasswordHash, DisplayName, IsActive, CreatedAt, SyncVersion, Version, SyncOperation)
    VALUES (@OrgCD, @Username, @PasswordHash, @DisplayName, @IsActive, @Now, @Now, 1, 'A');

    SET @Id = SCOPE_IDENTITY();
END
GO