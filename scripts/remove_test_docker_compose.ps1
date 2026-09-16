Push-Location (Split-Path -Parent $PSScriptRoot)
try {
    docker compose -f docker-compose.test.yml down --volumes --remove-orphans
    if ($LASTEXITCODE -ne 0) { throw "Disposable test stack failed to stop." }
} finally { Pop-Location }
