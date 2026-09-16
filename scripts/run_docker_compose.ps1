Push-Location (Split-Path -Parent $PSScriptRoot)
try {
    docker compose -f docker-compose.yml up --build -d
    if ($LASTEXITCODE -ne 0) { throw "Development stack failed to start." }
} finally { Pop-Location }
