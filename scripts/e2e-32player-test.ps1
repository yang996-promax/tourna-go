# E2E test: 32 players, Swiss -> Top 16 -> Champion
param(
    [int]$PlayerCount = 32,
    [int]$ExpectedSwissRounds = 5,
    [string]$TopCutSize = "Top16",
    [string]$MatchFormat = "BO3"
)

$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "Get-LocalSqlConfig.ps1")
$sqlServer = Get-TournaGoSqlServer
$sqlDatabase = Get-TournaGoSqlDatabase
$baseUrl = "http://localhost:5284"

function Invoke-Api {
    param([string]$HttpMethod, [string]$Path, $Body = $null, [hashtable]$Headers = @{})
    $params = @{
        Uri     = "$baseUrl$Path"
        Method  = $HttpMethod
        Headers = $Headers
    }
    if ($Body) {
        $params.ContentType = "application/json"
        $params.Body = ($Body | ConvertTo-Json -Depth 5 -Compress)
    }
    return Invoke-RestMethod @params
}

Write-Host "=== E2E: $PlayerCount players, $ExpectedSwissRounds Swiss rounds, $TopCutSize, $MatchFormat ===" -ForegroundColor Cyan

Write-Host "`n=== Step 1: Wipe all tournament data ===" -ForegroundColor Cyan
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
sqlcmd -S $sqlServer -d $sqlDatabase -C -Q $wipeSql | Out-Null
Write-Host "Database wiped."

Write-Host "`n=== Step 2: Login ===" -ForegroundColor Cyan
$login = Invoke-Api -HttpMethod POST -Path "/api/auth/login" -Body @{ username = "admin"; password = "admin123" }
$headers = @{ Authorization = "Bearer $($login.token)" }

Write-Host "`n=== Step 3: Create tournament ===" -ForegroundColor Cyan
$tournament = Invoke-Api -HttpMethod POST -Path "/api/tournaments" -Headers $headers -Body @{
    name                   = "E2E 32-Player Test"
    gameTitle              = "Pokemon TCG"
    eventDate              = (Get-Date).AddDays(7).ToString("yyyy-MM-dd")
    organizer              = "Test Organizer"
    venue                  = "Test Hall"
    totalSwissRounds       = $ExpectedSwissRounds
    topCutSize             = $TopCutSize
    firstRoundPairingMode  = "Random"
    matchFormat            = $MatchFormat
    hasElimination         = $false
}
$tournamentId = [int]$tournament.id
$apiRoot = "/api/tournaments/$($tournamentId)"
Write-Host "Created tournament #$tournamentId"

Write-Host "`n=== Step 4: Register $PlayerCount players ===" -ForegroundColor Cyan
for ($i = 0; $i -lt $PlayerCount; $i++) {
    $num = $i + 1
    Invoke-Api -HttpMethod POST -Path "$apiRoot/players" -Headers $headers -Body @{
        externalPlayerId = "P$num"
        name             = "Player $num"
        contactNumber    = "555-$(('{0:D4}' -f $num))"
        deckName         = "Deck $num"
    } | Out-Null
    if ($num % 8 -eq 0 -or $num -eq $PlayerCount) {
        Write-Host "  Registered $num / $PlayerCount"
    }
}

Write-Host "`n=== Step 5: Start tournament ===" -ForegroundColor Cyan
Invoke-Api -HttpMethod POST -Path "$apiRoot/start" -Headers $headers | Out-Null

$winResult = if ($MatchFormat -eq "BO1") { "Win2_0" } else { "Win2_0" }

Write-Host "`n=== Step 6: Play $ExpectedSwissRounds Swiss rounds ===" -ForegroundColor Cyan
$totalSwissMatches = 0
for ($roundNum = 1; $roundNum -le $ExpectedSwissRounds; $roundNum++) {
    $roundDto = Invoke-Api -HttpMethod POST -Path "$apiRoot/generate-round" -Headers $headers -Body @{}
    $played = 0
    foreach ($match in $roundDto.matches) {
        if ($match.isBye) { continue }
        Invoke-Api -HttpMethod POST -Path "/api/matches/$($match.id)/result" -Headers $headers -Body @{
            resultType = $winResult
        } | Out-Null
        $played++
        $totalSwissMatches++
    }
    $t = Invoke-Api -HttpMethod GET -Path $apiRoot -Headers $headers
    Write-Host "  Round $roundNum`: $($roundDto.matches.Count) tables, $played results - Status: $($t.status)"
}

Write-Host "`n=== Step 7: Standings (top 16) ===" -ForegroundColor Cyan
$standings = Invoke-Api -HttpMethod GET -Path "$apiRoot/standings" -Headers $headers
$standings | Select-Object -First 16 | ForEach-Object {
    Write-Host "  #$($_.rank) $($_.playerName) - $($_.matchPoints) MP ($($_.matchesWon)-$($_.matchesLost))"
}

Write-Host "`n=== Step 8: Generate $TopCutSize bracket ===" -ForegroundColor Cyan
$tree = Invoke-Api -HttpMethod POST -Path "$apiRoot/generate-topcut" -Headers $headers
Write-Host "  Brackets: $($tree.brackets.Count), Qualified: $($tree.qualifiedPlayers.Count)"

$topCutRoundOrder = @("RoundOf16", "QuarterFinal", "SemiFinal", "Final")

Write-Host "`n=== Step 9: Play top cut to champion ===" -ForegroundColor Cyan
$topCutResults = 0
foreach ($roundName in $topCutRoundOrder) {
    $tree = Invoke-Api -HttpMethod GET -Path "$apiRoot/topcut" -Headers $headers
    $playable = $tree.brackets | Where-Object {
        $_.round -eq $roundName -and -not $_.isComplete -and $_.playerAId -and $_.playerBId
    }
    if (-not $playable) { continue }

    $matchCount = @($playable).Count
    Write-Host "  --- $roundName ($matchCount matches) ---"
    foreach ($bracket in ($playable | Sort-Object matchPosition)) {
        Invoke-Api -HttpMethod POST -Path "/api/matches/topcut/$($bracket.id)/result" -Headers $headers -Body @{
            winnerId = $bracket.playerAId
        } | Out-Null
        Write-Host "    $($bracket.playerAName) def. $($bracket.playerBName)"
        $topCutResults++
    }
}

Write-Host "`n=== Step 10: Final verification ===" -ForegroundColor Cyan
$final = Invoke-Api -HttpMethod GET -Path "$apiRoot/topcut" -Headers $headers
$tournamentFinal = Invoke-Api -HttpMethod GET -Path $apiRoot -Headers $headers
$roundsUri = '{0}{1}/rounds' -f $baseUrl, $apiRoot
$authHeaders = @{ Authorization = "Bearer $($login.token)" }
$swissRounds = [object[]](Invoke-RestMethod -Uri ($roundsUri + '?swissOnly=true') -Method Get -Headers $authHeaders)
$allRounds = [object[]](Invoke-RestMethod -Uri $roundsUri -Method Get -Headers $authHeaders)
$swissRoundCount = ($swissRounds | Measure-Object).Count
$allRoundCount = ($allRounds | Measure-Object).Count

$activePlayers = @(Invoke-Api -HttpMethod GET -Path "$apiRoot/players" -Headers $headers).Count

Write-Host "  Tournament #$($tournamentFinal.id) status: $($tournamentFinal.status)"
Write-Host "  Champion: $($final.championName) (TP#$($final.championPlayerId))"
Write-Host "  Players registered: $PlayerCount, active in roster: $activePlayers"
Write-Host "  Swiss matches played: $totalSwissMatches"
Write-Host "  Top cut results entered: $topCutResults"
Write-Host "  Swiss rounds (pairings): $swissRoundCount — $(($swissRounds | ForEach-Object { $_.roundNumber }) -join ', ')"
Write-Host "  All DB rounds: $allRoundCount ($($allRoundCount - $swissRoundCount) top cut)"

$expectedTopCutRounds = 4  # R16, QF, SF, Final for Top16
$badSwiss = $swissRounds | Where-Object { $_.roundType -eq 'TopCut' }
$topCutComplete = @($allRounds | Where-Object { $_.roundType -eq 'TopCut' -and $_.isComplete }).Count

$ok = $final.championName `
    -and $tournamentFinal.status -eq "Completed" `
    -and $swissRoundCount -eq $ExpectedSwissRounds `
    -and $allRoundCount -eq ($ExpectedSwissRounds + $expectedTopCutRounds) `
    -and $badSwiss.Count -eq 0 `
    -and $topCutComplete -eq $expectedTopCutRounds `
    -and $tree.qualifiedPlayers.Count -eq 16

if ($ok) {
    Write-Host "`nSUCCESS: 32-player full flow complete — Champion $($final.championName)!" -ForegroundColor Green
    exit 0
}

Write-Host "`nFAILED: One or more checks did not pass." -ForegroundColor Red
Write-Host "  champion=$($final.championName) status=$($tournamentFinal.status) swissRounds=$($swissRounds.Count) allRounds=$($allRounds.Count) topCutComplete=$topCutComplete"
exit 1