# Resolve the .env path one level above
$envPath = Join-Path (Resolve-Path "..") ".env"

if (Test-Path $envPath) {
    Get-Content $envPath | ForEach-Object {
        if ($_ -match "^([^#=]+)=([^#]+)$") {
            $name = $matches[1].Trim()
            $value = $matches[2].Trim()
            [System.Environment]::SetEnvironmentVariable($name, $value)
        }
    }
} else {
    Write-Error ".env file not found at $envPath"
    exit 1
}

# Read environment variables
$projectKey = "PostsByMarko"
$sonarHostUrl = "http://localhost:9000/"
$sonarToken = $Env:SONAR_TOKEN

# Navigate to the Host project
cd ..
cd ./src/PostsByMarko.Host

# Sonar begin
dotnet sonarscanner begin `/k:"$projectKey" `/d:sonar.host.url="$sonarHostUrl" `/d:sonar.token="$sonarToken"

dotnet build

dotnet sonarscanner end /d:sonar.token="$sonarToken"

# Navigate back
cd ../..
cd ./scripts