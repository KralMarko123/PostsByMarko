Push-Location (Split-Path -Parent $PSScriptRoot)
try {
    docker compose -f docker-compose.test.yml up --build -d
    if ($LASTEXITCODE -ne 0) { throw "Disposable test stack failed to start." }
} finally { Pop-Location }
