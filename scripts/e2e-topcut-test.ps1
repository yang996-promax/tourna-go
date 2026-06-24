# E2E test: wipe DB, create event, Swiss -> Top Cut -> Champion
$ErrorActionPreference = "Stop"
$baseUrl = "http://localhost:5284"

function Invoke-Api {
    param([string]$Method, [string]$Path, $Body = $null, [hashtable]$Headers = @{})
    $params = @{
        Uri     = "$baseUrl$Path"
        Method  = $Method
        Headers = $Headers
    }
    if ($Body) {
        $params.ContentType = "application/json"
        $params.Body = ($Body | ConvertTo-Json -Depth 5 -Compress)
    }
    return Invoke-RestMethod @params
}

Write-Host "=== Step 1: Wipe all tournament data ===" -ForegroundColor Cyan
$wipeSql = @"
DELETE FROM MATCH_RESULT;
DELETE FROM MST_MATCH;
DELETE FROM BYE_HISTORY;
DELETE FROM TOURNAMENT_STANDING;
UPDATE TOP_CUT_BRACKET SET NextBracketId = NULL;
DELETE FROM TOP_CUT_BRACKET;
DELETE FROM MST_ROUND;
DELETE FROM TOURNAMENT_PLAYER;
DELETE FROM AUDIT_LOG;
DELETE FROM MST_TOURNAMENT;
DELETE FROM MST_PLAYER;
DBCC CHECKIDENT ('MST_TOURNAMENT', RESEED, 0);
DBCC CHECKIDENT ('MST_PLAYER', RESEED, 0);
DBCC CHECKIDENT ('TOURNAMENT_PLAYER', RESEED, 0);
DBCC CHECKIDENT ('MST_ROUND', RESEED, 0);
DBCC CHECKIDENT ('MST_MATCH', RESEED, 0);
DBCC CHECKIDENT ('MATCH_RESULT', RESEED, 0);
DBCC CHECKIDENT ('TOURNAMENT_STANDING', RESEED, 0);
DBCC CHECKIDENT ('BYE_HISTORY', RESEED, 0);
DBCC CHECKIDENT ('TOP_CUT_BRACKET', RESEED, 0);
DBCC CHECKIDENT ('AUDIT_LOG', RESEED, 0);
"@
sqlcmd -S ".\MSSQLSERVER_SIDE" -d TcgTournamentManager -C -Q $wipeSql | Out-Null
Write-Host "Database wiped (MST_ORGANIZER_USER preserved)."

Write-Host "`n=== Step 2: Login ===" -ForegroundColor Cyan
$login = Invoke-Api -Method POST -Path "/api/auth/login" -Body @{ username = "admin"; password = "admin123" }
$headers = @{ Authorization = "Bearer $($login.token)" }
Write-Host "Logged in as admin."

Write-Host "`n=== Step 3: Create tournament ===" -ForegroundColor Cyan
$tournament = Invoke-Api -Method POST -Path "/api/tournaments" -Headers $headers -Body @{
    name                   = "E2E Top Cut Test"
    gameTitle              = "Pokemon TCG"
    eventDate              = (Get-Date).AddDays(7).ToString("yyyy-MM-dd")
    organizer              = "Test Organizer"
    venue                  = "Test Hall"
    totalSwissRounds       = 3
    topCutSize             = "Top8"
    firstRoundPairingMode  = "Random"
}
Write-Host "Created tournament ID $($tournament.id): $($tournament.name)"

Write-Host "`n=== Step 4: Register 8 players ===" -ForegroundColor Cyan
$playerNames = @("Alpha", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot", "Golf", "Hotel")
$tpIds = @()
for ($i = 0; $i -lt 8; $i++) {
    $p = Invoke-Api -Method POST -Path "/api/tournaments/$($tournament.id)/players" -Headers $headers -Body @{
        externalPlayerId = "E2E-$($i + 1)"
        name             = $playerNames[$i]
        contactNumber    = "555-000$($i + 1)"
        deckName         = "Deck $($i + 1)"
    }
    $tpIds += $p.tournamentPlayerId
    Write-Host "  Registered $($p.name) (TP#$($p.tournamentPlayerId))"
}

Write-Host "`n=== Step 5: Start tournament ===" -ForegroundColor Cyan
Invoke-Api -Method POST -Path "/api/tournaments/$($tournament.id)/start" -Headers $headers | Out-Null
Write-Host "Tournament started."

Write-Host "`n=== Step 6: Play $($tournament.totalSwissRounds) Swiss rounds ===" -ForegroundColor Cyan
for ($round = 1; $round -le $tournament.totalSwissRounds; $round++) {
    $roundDto = Invoke-Api -Method POST -Path "/api/tournaments/$($tournament.id)/generate-round" -Headers $headers -Body @{}
    Write-Host "  Round $round generated ($($roundDto.matches.Count) matches)"

    foreach ($match in $roundDto.matches) {
        if ($match.isBye) {
            Write-Host "    Table $($match.tableNumber): BYE for $($match.playerAName)"
            continue
        }
        Invoke-Api -Method POST -Path "/api/matches/$($match.id)/result" -Headers $headers -Body @{
            resultType = "Win2_0"
        } | Out-Null
        Write-Host "    Table $($match.tableNumber): $($match.playerAName) beat $($match.playerBName) 2-0"
    }

    $t = Invoke-Api -Method GET -Path "/api/tournaments/$($tournament.id)" -Headers $headers
    Write-Host "  Round $round complete. Status: $($t.status), CurrentRound: $($t.currentRound)"
}

Write-Host "`n=== Step 7: Standings after Swiss ===" -ForegroundColor Cyan
$standings = Invoke-Api -Method GET -Path "/api/tournaments/$($tournament.id)/standings" -Headers $headers
$standings | Select-Object -First 8 | ForEach-Object {
    Write-Host "  #$($_.rank) $($_.playerName) - $($_.matchPoints) MP"
}

Write-Host "`n=== Step 8: Generate Top 8 bracket ===" -ForegroundColor Cyan
$tree = Invoke-Api -Method POST -Path "/api/tournaments/$($tournament.id)/generate-topcut" -Headers $headers
Write-Host "  Brackets created: $($tree.brackets.Count)"
Write-Host "  Qualified: $($tree.qualifiedPlayers.Count) players"

Write-Host "`n=== Step 9: Play top cut to champion ===" -ForegroundColor Cyan
$roundOrder = @("QuarterFinal", "SemiFinal", "Final")
foreach ($roundName in $roundOrder) {
    $playable = $tree.brackets | Where-Object { $_.round -eq $roundName -and -not $_.isComplete -and $_.playerAId -and $_.playerBId }
    if (-not $playable) { continue }

    Write-Host "  --- $roundName ---"
    foreach ($bracket in ($playable | Sort-Object matchPosition)) {
        $winnerId = $bracket.playerAId
        $winnerName = $bracket.playerAName
        Invoke-Api -Method POST -Path "/api/matches/topcut/$($bracket.id)/result" -Headers $headers -Body @{
            winnerId = $winnerId
        } | Out-Null
        Write-Host "    Match $($bracket.matchPosition): $winnerName def. $($bracket.playerBName)"
    }
    $tree = Invoke-Api -Method GET -Path "/api/tournaments/$($tournament.id)/topcut" -Headers $headers
}

Write-Host "`n=== Step 10: Final results ===" -ForegroundColor Cyan
$final = Invoke-Api -Method GET -Path "/api/tournaments/$($tournament.id)/topcut" -Headers $headers
$tournamentFinal = Invoke-Api -Method GET -Path "/api/tournaments/$($tournament.id)" -Headers $headers

Write-Host "  Tournament status: $($tournamentFinal.status)"
Write-Host "  Champion: $($final.championName) (TP#$($final.championPlayerId))"

Write-Host "`n=== Step 11: Pairings page filter (swissOnly) ===" -ForegroundColor Cyan
$swissRounds = Invoke-RestMethod -Uri "$baseUrl/api/tournaments/$($tournament.id)/rounds?swissOnly=true" -Headers $headers
$allRounds = Invoke-RestMethod -Uri "$baseUrl/api/tournaments/$($tournament.id)/rounds" -Headers $headers
Write-Host "  Swiss rounds on Pairings tab: $($swissRounds.Count) (numbers: $(($swissRounds.roundNumber -join ', ')))"
Write-Host "  All rounds in DB view: $($allRounds.Count) (includes $($allRounds.Count - $swissRounds.Count) top cut round(s))"
$badSwiss = $swissRounds | Where-Object { $_.roundType -eq 'TopCut' }
if ($badSwiss.Count -gt 0) {
    Write-Host "  FAIL: Top cut rounds leaked into swissOnly response" -ForegroundColor Red
    exit 1
}
Write-Host "  OK: No top cut rounds in pairings filter" -ForegroundColor Green

$ok = $final.championName -and $tournamentFinal.status -eq "Completed" -and $swissRounds.Count -eq 3 -and $allRounds.Count -gt $swissRounds.Count
if ($ok) {
    Write-Host "`nSUCCESS: Full event flow completed with champion $($final.championName)!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "`nFAILED: Flow or pairings filter check did not pass." -ForegroundColor Red
    exit 1
}