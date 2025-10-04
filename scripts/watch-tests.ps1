#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Runs tests in watch mode for the TextEncrypterDecrypter solution

.DESCRIPTION
    This script runs tests in watch mode, automatically re-running tests
    when source files change. Perfect for TDD development.

.PARAMETER Category
    Test category filter (unit, integration, ui, slow, external).

.PARAMETER Configuration
    Build configuration (Debug or Release). Defaults to Debug for faster iteration.

.EXAMPLE
    .\watch-tests.ps1
    Runs all tests in watch mode.

.EXAMPLE
    .\watch-tests.ps1 -Category unit
    Runs only unit tests in watch mode.
#>

param(
    [ValidateSet("unit", "integration", "ui", "slow", "external")]
    [string]$Category,
    
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug"
)

$ErrorActionPreference = "Stop"

Write-Host "Starting test watch mode for TextEncrypterDecrypter..." -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow

if ($Category) {
    Write-Host "Category: $Category" -ForegroundColor Yellow
    Write-Host "Watching for changes in $Category tests..." -ForegroundColor Cyan
} else {
    Write-Host "Watching for changes in all tests..." -ForegroundColor Cyan
}

Write-Host "Press Ctrl+C to stop watching" -ForegroundColor Yellow
Write-Host ""

$watchArgs = @("watch", "test", "--configuration", $Configuration)

if ($Category) {
    $watchArgs += @("--filter", "Category=$Category")
}

Write-Host "Executing: dotnet $($watchArgs -join ' ')" -ForegroundColor Cyan
Write-Host ""

& dotnet $watchArgs
