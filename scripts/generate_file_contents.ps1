cd ..
cd ./src/PostsByMarko.Host

# Allowed file extensions (C# logic + project files)
$includeExtensions = @(".cs", ".csproj", ".sln", ".json", ".razor")

# Folders to exclude (generated/noise)
$excludeDirs = @("bin", "obj", "node_modules", ".vs")

# Patterns to exclude (generated C# files)
$excludePatterns = @("*\.g.cs", "*\.g.i.cs", "*Designer.cs", "*AssemblyInfo.cs")

# Delimiter for readability in output
$delimiter = "`n`n===== FILE START =====`n`n"

# StringBuilder is efficient for large concatenation
$stringBuilder = New-Object System.Text.StringBuilder

# Get all files recursively while excluding noise folders
$files = Get-ChildItem -Recurse -File | Where-Object {
    # Extension must match
    $includeExtensions -contains $_.Extension.ToLower() -and
    # Parent folder must not be excluded
    ($excludeDirs -notcontains $_.Directory.Name) -and
    # File must not match excluded patterns
    ($excludePatterns -notcontains $_.Name)
}

foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw

    $null = $stringBuilder.Append($delimiter)
    $null = $stringBuilder.AppendLine("PATH: $($file.FullName)")
    $null = $stringBuilder.AppendLine("CONTENT:")
    $null = $stringBuilder.AppendLine($content)
}

# Determine output path (two levels above current directory)
$basePath = (Get-Location).Path
$twoLevelsUp = Split-Path (Split-Path $basePath)
$outputPath = Join-Path $twoLevelsUp "contents.txt"

# Write the collected content
Set-Content -Path $outputPath -Value $stringBuilder.ToString()

cd ../..
cd ./scripts