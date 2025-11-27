cd ..
cd ./src/PostsByMarko.Host

# Define delimiter between file contents
$delimiter = "`n`n===== FILE START =====`n`n"

# Prepare a StringBuilder for efficiency
$stringBuilder = New-Object System.Text.StringBuilder

# Get all files recursively starting from current directory
$files = Get-ChildItem -Path . -Recurse -File

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw

    $null = $stringBuilder.Append($delimiter)
    $null = $stringBuilder.AppendLine("PATH: $($file.FullName)")
    $null = $stringBuilder.AppendLine("CONTENT:")
    $null = $stringBuilder.AppendLine($content)
}

# Determine output file path (two levels above)
$basePath = (Get-Location).Path
$twoLevelsUp = Split-Path (Split-Path $basePath)

$outputPath = Join-Path $twoLevelsUp "contents.txt"

# Write everything to contents.txt
Set-Content -Path $outputPath -Value $stringBuilder.ToString()

# Optional: inform the user
Write-Host "File written to: $outputPath"

cd ../..
cd ./scripts