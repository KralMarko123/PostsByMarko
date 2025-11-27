# Go to repo root (relative to scripts folder)
Set-Location "$PSScriptRoot/.."

# Ensure ReportGenerator is installed globally
dotnet tool install -g dotnet-reportgenerator-globaltool | Out-Null

# Make sure PowerShell knows where to find global .NET tools
$env:PATH += ";$env:USERPROFILE\.dotnet\tools"

$unitTestsProj = "test/PostsByMarko.UnitTests/PostsByMarko.UnitTests.csproj"
$integrationTestsProj = "test/PostsByMarko.IntegrationTests/PostsByMarko.IntegrationTests.csproj"
$coverageOutputDir = "TestResults/Coverage"
$reportOutputDir = "$coverageOutputDir/Report"

Write-Host "=== Running Unit Tests with Coverage ==="
dotnet test $unitTestsProj `
    --settings coverage.runsettings `
    --collect "XPlat Code Coverage" `
    --results-directory "$coverageOutputDir/Unit"

Write-Host "=== Running Integration Tests with Coverage ==="
dotnet test $integrationTestsProj `
    --settings coverage.runsettings `
    --collect "XPlat Code Coverage" `
    --results-directory "$coverageOutputDir/Integration"

Write-Host "=== Generating merged and filtered coverage report ==="
reportgenerator `
    -reports:"$coverageOutputDir/Unit/**/coverage.cobertura.xml;$coverageOutputDir/Integration/**/coverage.cobertura.xml" `
    -targetdir:"$reportOutputDir" `
    -reporttypes:"Html;Cobertura" `
    -classfilters:"-*.Builders.*;-*.Constants.*;-*.Migrations.*;-*.Extensions.*;-*.Exceptions.*;-*.Hubs.*;-*.Middlewares.*;-*.Helper.*"

Write-Host "`n✅ Coverage report generated under: $reportOutputDir"

# Optional: only open report on local Windows
if ($env:OS -eq "Windows_NT") {
    Start-Process "$reportOutputDir/index.html"
}
