cd ..
dotnet tool install -g dotnet-reportgenerator-globaltool

$dir = pwd

Remove-Item -Recurse -Force $dir/TestResults/

$output = [string] (& dotnet test ./PostsByMarko.sln --collect:"XPlat Code Coverage" 2>&1)
Write-Host "Last Exit Code: $lastexitcode"
Write-Host $output

Remove-Item -Recurse -Force $dir/coveragereport/

reportgenerator -reports:"$dir/**/coverage.cobertura.xml" -targetdir:"$dir/coveragereport" -reporttypes:Html -historydir:$dir/CoverageHistory 

$osInfo = Get-CimInstance -ClassName Win32_OperatingSystem
if ($osInfo.ProductType -eq 1) {
    (& "$dir/coveragereport/index.html")
}

cd ./scripts