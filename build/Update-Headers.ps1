# Script to Update Header comment in C# sources within this repository.

# This script duplicates the fuctionality provided by the 'UpdateHeaders' target in Cake script present previously.
# Since, Cake build has been removed, this fuctionality has been implimented here in this PowerShell script.

[CmdletBinding()]
Param(
    [Alias("Verify")]
    [switch]$DryRun,
    [switch]$Clean
)

function Clear-BuildArtifacts {
    if (Get-Command dotnet) {
        Write-Host "Starting 'dotnet msbuild -t:Clean' to clean-up build outputs"
        dotnet msbuild -noLogo -noAutoRsp -v:m -t:Clean
    }
    elseif (Get-Command msbuild) {
        Write-Host "Starting 'msbuild -t:Clean' to clean-up build outputs"
        msbuild -noLogo -noAutoRsp -v:m -t:Clean
    }
    elseif (Get-Command git) {
        Write-Host "Starting 'git clean -Xdf' to clean-up build outputs"
        git clean -Xdf
    }
    else {
        Write-Warning "Can't find dotnet/msbuild/git to clean-up build outputs"
    }
}

function Get-SourceFiles ([string]$Path, [string]$Extension) {
    $fileType = $Extension.TrimStart('.')
    $fileFilter = "*.$fileType"
    $fileExcludes = "*.g.$fileType", "*.i.$fileType", "*TemporaryGeneratedFile*.$fileType"
    $sourceFiles = Get-ChildItem -Path $Path -File -Recurse -Filter $fileFilter -Exclude $fileExcludes
    return $sourceFiles.Where({ !($_.FullName.Contains("\bin\") -or $_.FullName.Contains("\obj\")) })
}

# Set Repot Root
$repoRoot = Split-Path -Path $PSScriptRoot -Parent
Push-Location $repoRoot

# Clean-up Repository build outputs
if ($Clean) {
    Clear-BuildArtifacts
}

# Get C# source files recursively
$sourceFiles = Get-SourceFiles -Path $repoRoot -Extension ".cs"

Write-Host "Checking $($sourceFiles.Count) C# file header(s)"

$hasMissing = $false
foreach ($sourceFile in $sourceFiles) {

    $oldContent = Get-Content $sourceFile -Raw | Out-String -NoNewline

    if ($oldContent.Contains("// <auto-generated/>") -or $oldContent.Contains("// <auto-generated>")) {
        continue
    }

    $headerFilePath = Join-Path $PSScriptRoot "header.txt"
    $headerContent = Get-Content $headerFilePath -Raw | Out-String
    $regexHeader = "^(//.*\r?\n)*\r?\n"
    $newContent = $oldContent -replace $regexHeader,$headerContent | Out-String -NoNewline

    if (-not ($newContent -eq $oldContent)) {
        $sourceFilePath = [System.IO.Path]::GetRelativePath($repoRoot, $sourceFile.FullName)

        if ($DryRun) {
            Write-Warning "Wrong/missing file header in '$sourceFilePath'"
            $hasMissing = $true
        }
        else {
            Write-Host "Updating '$sourceFilePath' file header..."
            Set-Content -Path $sourceFile -Value $newContent -NoNewline
        }
    }
}

if ($DryRun -and $hasMissing) {
    Write-Error "Please run 'Update-Headers.ps1' to verify and add missing headers ins source files before commiting your changes."
}
else {
    Write-Host "All $($sourceFiles.Count) C# file header(s) are up to date."
}

# Pop out of Repo Root
Pop-Location
