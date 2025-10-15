# Go to repo root
cd ..

# Ensure ReportGenerator is installed globally
dotnet tool install -g dotnet-reportgenerator-globaltool | Out-Null

# Add global .NET tools path to current session
$env:PATH += ";$env:USERPROFILE\.dotnet\tools"

$unitTestsProj = "test/PostsByMarko.UnitTests/PostsByMarko.UnitTests.csproj"
$integrationTestsProj = "test/PostsByMarko.IntegrationTests/PostsByMarko.IntegrationTests.csproj"
$coverageOutputDir = "TestResults/Coverage"
$reportOutputDir = "$coverageOutputDir\Report"

Write-Host "=== Running Unit Tests with Coverage ==="
dotnet test $unitTestsProj `
    --settings coverage.runsettings `
    --collect "XPlat Code Coverage" `
    --results-directory "$coverageOutputDir\Unit"

Write-Host "=== Running Integration Tests with Coverage ==="
dotnet test $integrationTestsProj `
    --settings coverage.runsettings `
    --collect "XPlat Code Coverage" `
    --results-directory "$coverageOutputDir\Integration"

Write-Host "=== Generating merged and filtered coverage report ==="

# Combine coverage results and filter out unwanted namespaces
reportgenerator `
    -reports:"$coverageOutputDir\Unit\**\coverage.cobertura.xml;$coverageOutputDir\Integration\**\coverage.cobertura.xml" `
    -targetdir:"$reportOutputDir" `
    -reporttypes:Html `
    -assemblyfilters:"+PostsByMarko.*;-PostsByMarko.Host.Builders*;-PostsByMarko.Host.Constants*;-PostsByMarko.Host.Data*;-PostsByMarko.Host.Extensions*;-PostsByMarko.Host.Hubs*;-PostsByMarko.Host.Middlewares*" `
    -classfilters:"-*.Builders.*;-*.Constants.*;-*.Data.*;-*.Extensions.*;-*.Hubs.*;-*.Middlewares.*"

Write-Host "`n✅ Coverage report generated under: $reportOutputDir"

# Optional: open in browser (only if you're on Windows)
if ($env:OS -eq "Windows_NT") {
    Start-Process "$reportOutputDir\index.html"
}

# Return to scripts directory
cd ./scripts
