param([int]$Id = 14)
$ErrorActionPreference = "Stop"
$baseUrl = "http://localhost:5284"
$login = Invoke-RestMethod -Uri "$baseUrl/api/auth/login" -Method POST -Body (@{ username = "admin"; password = "admin123" } | ConvertTo-Json) -ContentType "application/json"
$h = @{ Authorization = "Bearer $($login.token)" }
$swiss = Invoke-RestMethod -Uri "$baseUrl/api/tournaments/$Id/rounds" -Method GET -Headers $h
$swissOnly = Invoke-RestMethod -Uri "$baseUrl/api/tournaments/${Id}/rounds" -Method GET -Headers $h -Body @{ swissOnly = $true } -ErrorAction SilentlyContinue
# query via URI
$swissOnly2 = Invoke-RestMethod -Uri ('{0}/api/tournaments/{1}/rounds?swissOnly=true' -f $baseUrl, $Id) -Method GET -Headers $h
$topcut = Invoke-RestMethod -Uri "$baseUrl/api/tournaments/$Id/topcut" -Method GET -Headers $h
$t = Invoke-RestMethod -Uri "$baseUrl/api/tournaments/$Id" -Method GET -Headers $h
Write-Host "Tournament $Id : $($t.status) | Champion: $($topcut.championName)"
Write-Host "Rounds all: $(@($swiss).Count) swissOnly: $(@($swissOnly2).Count)"