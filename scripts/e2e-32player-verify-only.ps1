param([int]$TournamentId = 16)
$ErrorActionPreference = "Stop"
$baseUrl = "http://localhost:5284"
$ExpectedSwissRounds = 5
$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body (@{ username = "admin"; password = "admin123" } | ConvertTo-Json) -ContentType "application/json"
$authHeaders = @{ Authorization = "Bearer $($login.token)" }
$apiRoot = "/api/tournaments/$TournamentId"
$roundsUri = '{0}{1}/rounds' -f $baseUrl, $apiRoot
$swissUri = '{0}/api/tournaments/{1}/rounds?swissOnly=true' -f $baseUrl, $TournamentId
$swissRounds = [object[]](Invoke-RestMethod -Uri $swissUri -Method Get -Headers $authHeaders)
$allRounds = [object[]](Invoke-RestMethod -Uri $roundsUri -Method Get -Headers $authHeaders)
$final = Invoke-RestMethod -Uri "$baseUrl$apiRoot/topcut" -Method Get -Headers $authHeaders
$tournamentFinal = Invoke-RestMethod -Uri "$baseUrl$apiRoot" -Method Get -Headers $authHeaders
$expectedTopCutRounds = 4
$topCutComplete = @($allRounds | Where-Object { $_.roundType -eq 'TopCut' -and $_.isComplete }).Count
$swissRoundCount = ($swissRounds | Measure-Object).Count
$allRoundCount = ($allRounds | Measure-Object).Count
$ok = $final.championName -and $tournamentFinal.status -eq "Completed" -and $swissRoundCount -eq $ExpectedSwissRounds -and $allRoundCount -eq ($ExpectedSwissRounds + $expectedTopCutRounds) -and $topCutComplete -eq $expectedTopCutRounds
Write-Host "Tournament $TournamentId : $($tournamentFinal.status) | Champion: $($final.championName)"
Write-Host "Swiss rounds: $swissRoundCount | All rounds: $allRoundCount | TopCut complete: $topCutComplete"
if ($ok) { Write-Host "VERIFY OK" -ForegroundColor Green; exit 0 } else { Write-Host "VERIFY FAIL" -ForegroundColor Red; exit 1 }