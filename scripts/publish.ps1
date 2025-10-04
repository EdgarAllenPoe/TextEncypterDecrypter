#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Publishes the TextEncrypterDecrypter application

.DESCRIPTION
    This script publishes the application using predefined publish profiles.
    Supports both Portable and NativeAOT configurations.

.PARAMETER Profile
    Publish profile (Portable or NativeAot). Defaults to Portable.

.PARAMETER Configuration
    Build configuration (Debug or Release). Defaults to Release.

.PARAMETER Open
    Open the publish directory after publishing.

.EXAMPLE
    .\publish.ps1
    Publishes the application using Portable profile.

.EXAMPLE
    .\publish.ps1 -Profile NativeAot
    Publishes the application using NativeAOT profile.

.EXAMPLE
    .\publish.ps1 -Profile Portable -Open
    Publishes and opens the output directory.
#>

param(
    [ValidateSet("Portable", "NativeAot")]
    [string]$Profile = "Portable",
    
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [switch]$Open
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

Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --configuration $Configuration
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Host "Publishing application..." -ForegroundColor Yellow
$publishArgs = @("publish", $projectPath, "-p:PublishProfile=$Profile", "--configuration", $Configuration)
& dotnet $publishArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "Publish failed"
    exit 1
}

$publishDir = "src/TextEncrypterDecrypter.App/bin/$Configuration/net8.0/publish/$Profile"
if (Test-Path $publishDir) {
    Write-Host "Publish completed successfully!" -ForegroundColor Green
    Write-Host "Output directory: $publishDir" -ForegroundColor Cyan
    
    # Show publish directory contents
    Write-Host "`nPublish directory contents:" -ForegroundColor Yellow
    Get-ChildItem $publishDir | Format-Table Name, Length, LastWriteTime
    
    if ($Open) {
        Write-Host "`nOpening publish directory..." -ForegroundColor Yellow
        Start-Process explorer.exe $publishDir
    }
} else {
    Write-Warning "Publish directory not found: $publishDir"
}

Write-Host "`nPublish completed!" -ForegroundColor Green
