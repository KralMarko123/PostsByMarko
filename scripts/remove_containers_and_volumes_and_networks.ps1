# Preserve persistent development data, despite this script's legacy filename.
Push-Location (Split-Path -Parent $PSScriptRoot)
try {
    docker compose -f docker-compose.yml down --remove-orphans
    if ($LASTEXITCODE -ne 0) { throw "Development stack failed to stop." }
} finally { Pop-Location }
