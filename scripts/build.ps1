#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds the TextEncrypterDecrypter solution

.DESCRIPTION
    This script builds the entire solution with appropriate configuration.
    Supports both Debug and Release configurations with verbose output.

.PARAMETER Configuration
    Build configuration (Debug or Release). Defaults to Release.

.PARAMETER Clean
    Clean before building.

.PARAMETER Verbose
    Enable verbose output.

.PARAMETER NoRestore
    Skip package restore.

.EXAMPLE
    .\build.ps1
    Builds the solution in Release configuration.

.EXAMPLE
    .\build.ps1 -Configuration Debug -Clean -Verbose
    Cleans and builds the solution in Debug configuration with verbose output.
#>

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [switch]$Clean,
    [switch]$Verbose,
    [switch]$NoRestore
)

$ErrorActionPreference = "Stop"

# Ensure we're in the correct directory
if (-not (Test-Path "TextEncrypterDecrypter.sln")) {
    Write-Error "Solution file not found. Please run this script from the solution root directory."
    exit 1
}

Write-Host "🔨 Building TextEncrypterDecrypter solution..." -ForegroundColor Green
Write-Host "📦 Configuration: $Configuration" -ForegroundColor Yellow

if ($Clean) {
    Write-Host "🧹 Cleaning solution..." -ForegroundColor Yellow
    $cleanArgs = @("clean", "--configuration", $Configuration)
    if ($Verbose) { $cleanArgs += "--verbosity", "normal" }
    
    & dotnet $cleanArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Clean failed"
        exit 1
    }
}

if (-not $NoRestore) {
    Write-Host "📥 Restoring packages..." -ForegroundColor Yellow
    $restoreArgs = @("restore")
    if ($Verbose) { $restoreArgs += "--verbosity", "normal" }
    
    & dotnet $restoreArgs
    if ($LASTEXITCODE -ne 0) {
        Write-Error "❌ Restore failed"
        exit 1
    }
}

Write-Host "🔨 Building solution..." -ForegroundColor Yellow
$buildArgs = @("build", "--configuration", $Configuration, "--no-restore")
if ($Verbose) { $buildArgs += "--verbosity", "normal" }

& dotnet $buildArgs
if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Build failed"
    exit 1
}

Write-Host "✅ Build completed successfully!" -ForegroundColor Green
