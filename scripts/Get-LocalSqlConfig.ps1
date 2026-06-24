function Get-TournaGoSqlServer {
    if ($env:TOURNA_GO_SQL_SERVER) {
        return $env:TOURNA_GO_SQL_SERVER
    }

    $localConfig = Join-Path $PSScriptRoot "_local-config.ps1"
    if (Test-Path $localConfig) {
        . $localConfig
        if ($script:SqlServer) {
            return $script:SqlServer
        }
    }

    throw @"
SQL Server is not configured for scripts.
Copy scripts/_local-config.ps1.example to scripts/_local-config.ps1
or set environment variable TOURNA_GO_SQL_SERVER.
"@
}

function Get-TournaGoSqlDatabase {
    if ($env:TOURNA_GO_SQL_DATABASE) {
        return $env:TOURNA_GO_SQL_DATABASE
    }

    $localConfig = Join-Path $PSScriptRoot "_local-config.ps1"
    if (Test-Path $localConfig) {
        . $localConfig
        if ($script:SqlDatabase) {
            return $script:SqlDatabase
        }
    }

    return "TcgTournamentManager"
}