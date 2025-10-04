#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Builds the TextEncrypterDecrypter solution

.DESCRIPTION
    This script builds the entire solution with appropriate configuration.
    Supports both Debug and Release configurations.

.PARAMETER Configuration
    Build configuration (Debug or Release). Defaults to Release.

.PARAMETER Clean
    Clean before building.

.EXAMPLE
    .\build.ps1
    Builds the solution in Release configuration.

.EXAMPLE
    .\build.ps1 -Configuration Debug -Clean
    Cleans and builds the solution in Debug configuration.
#>

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [switch]$Clean
)

$ErrorActionPreference = "Stop"

Write-Host "Building TextEncrypterDecrypter solution..." -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow

if ($Clean) {
    Write-Host "Cleaning solution..." -ForegroundColor Yellow
    dotnet clean --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Clean failed"
        exit 1
    }
}

Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Restore failed"
    exit 1
}

Write-Host "Building solution..." -ForegroundColor Yellow
dotnet build --configuration $Configuration --no-restore
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

Write-Host "Build completed successfully!" -ForegroundColor Green
