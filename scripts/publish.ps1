#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publishes the TextEncrypterDecrypter application

.DESCRIPTION
    This script publishes the application using predefined publish profiles.
    Supports both Portable and NativeAOT configurations with enhanced output.

.PARAMETER Profile
    Publish profile (Portable or NativeAot). Defaults to Portable.

.PARAMETER Configuration
    Build configuration (Debug or Release). Defaults to Release.

.PARAMETER Open
    Open the publish directory after publishing.

.PARAMETER Verbose
    Enable verbose output.

.PARAMETER Clean
    Clean before publishing.

.EXAMPLE
    .\publish.ps1
    Publishes the application using Portable profile.

.EXAMPLE
    .\publish.ps1 -Profile NativeAot -Verbose
    Publishes the application using NativeAOT profile with verbose output.

.EXAMPLE
    .\publish.ps1 -Profile Portable -Open -Clean
    Cleans, publishes, and opens the output directory.
#>

param(
    [ValidateSet("Portable", "NativeAot")]
    [string]$Profile = "Portable",
    
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [switch]$Open,
    [switch]$Verbose,
    [switch]$Clean
)

$ErrorActionPreference = "Stop"

Write-Host "Publishing TextEncrypterDecrypter application..." -ForegroundColor Green
Write-Host "Profile: $Profile" -ForegroundColor Yellow
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow

# Ensure we're in the correct directory
$projectPath = "src/TextEncrypterDecrypter.App/TextEncrypterDecrypter.App.csproj"
if (-not (Test-Path $projectPath)) {
    Write-Error "Project file not found: $projectPath"
    Write-Error "Please run this script from the solution root directory"
    exit 1
}

if ($Clean) {
    Write-Host "Cleaning solution..." -ForegroundColor Yellow
    $cleanArgs = @("clean", "--configuration", $Configuration)
    if ($Verbose) { $cleanArgs += "--verbosity", "normal" }
    
    & dotnet $cleanArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Clean failed"
        exit 1
    }
}

Write-Host "Building solution..." -ForegroundColor Yellow
$buildArgs = @("build", "--configuration", $Configuration)
if ($Verbose) { $buildArgs += "--verbosity", "normal" }

& dotnet $buildArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Host "Publishing application..." -ForegroundColor Yellow
$publishArgs = @("publish", $projectPath, "-p:PublishProfile=$Profile", "--configuration", $Configuration)
if ($Verbose) { $publishArgs += "--verbosity", "normal" }

& dotnet $publishArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed"
    exit 1
}

# Try to find the publish directory - it might be in different locations depending on the profile
$possiblePaths = @(
    "src/TextEncrypterDecrypter.App/bin/$Configuration/net8.0/publish/$Profile",
    "src/TextEncrypterDecrypter.App/bin/$Configuration/net8.0/win-x64/publish/$Profile",
    "src/TextEncrypterDecrypter.App/bin/$Configuration/net8.0/publish"
)

$publishDir = $null
foreach ($path in $possiblePaths) {
    if (Test-Path $path) {
        $publishDir = $path
        break
    }
}

if ($publishDir) {
    Write-Host ""
    Write-Host "Publish completed successfully!" -ForegroundColor Green
    Write-Host "Output directory: $publishDir" -ForegroundColor Cyan
    
    # Show publish directory contents with better formatting
    Write-Host ""
    Write-Host "Publish directory contents:" -ForegroundColor Yellow
    $files = Get-ChildItem $publishDir
    if ($files.Count -gt 0) {
        $files | ForEach-Object {
            $size = if ($_.Length -gt 1MB) { 
                "{0:N1} MB" -f ($_.Length / 1MB) 
            } elseif ($_.Length -gt 1KB) { 
                "{0:N1} KB" -f ($_.Length / 1KB) 
            } else { 
                "$($_.Length) B" 
            }
            Write-Host "   $($_.Name) ($size)" -ForegroundColor White
        }
        
        # Show total size
        $totalSize = ($files | Measure-Object -Property Length -Sum).Sum
        $totalSizeFormatted = if ($totalSize -gt 1MB) { 
            "{0:N1} MB" -f ($totalSize / 1MB) 
        } elseif ($totalSize -gt 1KB) { 
            "{0:N1} KB" -f ($totalSize / 1KB) 
        } else { 
            "$totalSize B" 
        }
        Write-Host ""
        Write-Host "Total size: $totalSizeFormatted" -ForegroundColor Cyan
    }
    
    if ($Open) {
        Write-Host ""
        Write-Host "Opening publish directory..." -ForegroundColor Yellow
        Start-Process explorer.exe $publishDir
    }
} else {
    Write-Warning "Publish directory not found. Checked paths:"
    foreach ($path in $possiblePaths) {
        Write-Warning "   • $path"
    }
}

Write-Host ""
Write-Host "Publish completed!" -ForegroundColor Green
