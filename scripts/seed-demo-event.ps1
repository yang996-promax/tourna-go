# Creates a demo tournament with 8 players ready to start.
$ErrorActionPreference = "Stop"
$baseUrl = "http://localhost:5284"

$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -ContentType "application/json" -Body '{"username":"admin","password":"admin123"}'
$headers = @{ Authorization = "Bearer $($login.token)" }

$tournament = Invoke-RestMethod -Uri "$baseUrl/api/tournaments" -Method POST -ContentType "application/json" -Headers $headers -Body (@{
    name = "Demo Tournament"
    gameTitle = "Pokemon TCG"
    eventDate = (Get-Date).AddDays(3).ToString("yyyy-MM-dd")
    organizer = "Tourna-Go"
    venue = "Main Hall"
    totalSwissRounds = 3
    topCutSize = "Top8"
    firstRoundPairingMode = "Random"
    matchFormat = "BO3"
    hasElimination = $false
} | ConvertTo-Json)

$players = @(
    @{ externalPlayerId = "D1"; name = "Player One"; contactNumber = "555-0001"; deckName = "Deck A" },
    @{ externalPlayerId = "D2"; name = "Player Two"; contactNumber = "555-0002"; deckName = "Deck B" },
    @{ externalPlayerId = "D3"; name = "Player Three"; contactNumber = "555-0003"; deckName = "Deck C" },
    @{ externalPlayerId = "D4"; name = "Player Four"; contactNumber = "555-0004"; deckName = "Deck D" },
    @{ externalPlayerId = "D5"; name = "Player Five"; contactNumber = "555-0005"; deckName = "Deck E" },
    @{ externalPlayerId = "D6"; name = "Player Six"; contactNumber = "555-0006"; deckName = "Deck F" },
    @{ externalPlayerId = "D7"; name = "Player Seven"; contactNumber = "555-0007"; deckName = "Deck G" },
    @{ externalPlayerId = "D8"; name = "Player Eight"; contactNumber = "555-0008"; deckName = "Deck H" }
)

foreach ($p in $players) {
    Invoke-RestMethod -Uri "$baseUrl/api/tournaments/$($tournament.id)/players" -Method POST -ContentType "application/json" -Headers $headers -Body ($p | ConvertTo-Json) | Out-Null
}

Write-Host "Created Demo Tournament #$($tournament.id) with 8 players (Registration)." -ForegroundColor Green
Write-Host "Open: http://localhost:5173/events/$($tournament.id)" -ForegroundColor Cyan