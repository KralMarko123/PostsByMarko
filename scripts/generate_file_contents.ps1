# From scripts/, go one level up to repo root
$root = Split-Path -Path $PSScriptRoot -Parent
Set-Location $root

# Common settings
$includeExtensions = @(
    ".cs", ".csproj", ".sln", ".json",
    ".razor", ".css", ".html",
    ".js", ".jsx", ".ts", ".tsx"
)

# Folders to exclude (.NET + frontend noise)
$excludeDirs = @("bin", "obj", ".vs", "node_modules", ".sonarqube")

# Patterns to exclude (generated / lock files)
$excludePatterns = @(
    "*.g.cs",
    "*.g.i.cs",
    "*Designer.cs",
    "*AssemblyInfo.cs",
    "package-lock.json"
)

$delimiter = "`n`n===== FILE START =====`n`n"

function IsUnderExcludedDir {
    param(
        [System.IO.FileInfo] $File,
        [string[]] $DirsToExclude
    )

    $dir = $File.Directory
    while ($dir) {
        if ($DirsToExclude -contains $dir.Name) { return $true }
        $dir = $dir.Parent
    }
    return $false
}

function Write-ProjectContents {
    param(
        [string] $ProjectRoot,
        [string] $OutputPath
    )

    $stringBuilder = New-Object System.Text.StringBuilder

    $files = Get-ChildItem -Path $ProjectRoot -Recurse -File | Where-Object {
        $file = $_

        # Extension must be in the include list
        ($includeExtensions -contains $file.Extension.ToLower()) -and

        # Not inside excluded directories
        -not (IsUnderExcludedDir -File $file -DirsToExclude $excludeDirs) -and

        # Name doesn't match any excluded wildcard pattern
        -not ($excludePatterns | Where-Object { $file.Name -like $_ })
    }

    foreach ($file in $files) {
        $content = Get-Content -Path $file.FullName -Raw

        $null = $stringBuilder.Append($delimiter)
        $null = $stringBuilder.AppendLine("PATH: $($file.FullName)")
        $null = $stringBuilder.AppendLine("CONTENT:")
        $null = $stringBuilder.AppendLine($content)
    }

    # Overwrite the file each run
    Set-Content -Path $OutputPath -Value $stringBuilder.ToString() -Encoding UTF8
}

# Backend: src/PostsByMarko.Host -> backend.txt
$backendRoot = Join-Path $root "src/PostsByMarko.Host"
$backendOut  = Join-Path $root "backend.txt"
Write-ProjectContents -ProjectRoot $backendRoot -OutputPath $backendOut

# Frontend: src/PostsByMarko.Client -> frontend.txt
$frontendRoot = Join-Path $root "src/PostsByMarko.Client"
$frontendOut  = Join-Path $root "frontend.txt"
Write-ProjectContents -ProjectRoot $frontendRoot -OutputPath $frontendOut

# Return to scripts directory at the end
Set-Location $PSScriptRoot
