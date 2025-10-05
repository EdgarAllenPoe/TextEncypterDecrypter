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

.PARAMETER Verbose
    Enable verbose output.

.PARAMETER Project
    Specific project to watch (relative path to .csproj file).

.EXAMPLE
    .\watch-tests.ps1
    Runs all tests in watch mode.

.EXAMPLE
    .\watch-tests.ps1 -Category unit
    Runs only unit tests in watch mode.

.EXAMPLE
    .\watch-tests.ps1 -Project tests/TextEncrypterDecrypter.UnitTests/TextEncrypterDecrypter.UnitTests.csproj
    Watches a specific test project.
#>

param(
    [ValidateSet("unit", "integration", "ui", "slow", "external")]
    [string]$Category,
    
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Debug",
    
    [switch]$Verbose,
    
    [string]$Project
)

$ErrorActionPreference = "Stop"

# Ensure we're in the correct directory
if (-not (Test-Path "TextEncrypterDecrypter.sln")) {
    Write-Error "Solution file not found. Please run this script from the solution root directory."
    exit 1
}

Write-Host "Starting test watch mode for TextEncrypterDecrypter..." -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow

if ($Category) {
    Write-Host "Category: $Category" -ForegroundColor Yellow
    Write-Host "Watching for changes in $Category tests..." -ForegroundColor Cyan
} else {
    Write-Host "Watching for changes in all tests..." -ForegroundColor Cyan
}

if ($Project) {
    Write-Host "Project: $Project" -ForegroundColor Yellow
    if (-not (Test-Path $Project)) {
        Write-Error "Project file not found: $Project"
        exit 1
    }
}

Write-Host ""
Write-Host "TDD Tips:" -ForegroundColor Magenta
Write-Host "   • Write a failing test first (RED)" -ForegroundColor White
Write-Host "   • Make it pass with minimal code (GREEN)" -ForegroundColor White
Write-Host "   • Refactor while keeping tests green (REFACTOR)" -ForegroundColor White
Write-Host ""
Write-Host "Press Ctrl+C to stop watching" -ForegroundColor Yellow
Write-Host ""

$watchArgs = @("watch", "test", "--configuration", $Configuration)

if ($Project) {
    $watchArgs += $Project
}

if ($Category) {
    $watchArgs += @("--filter", "Category=$Category")
}

if ($Verbose) {
    $watchArgs += @("--verbosity", "normal")
}

Write-Host "Executing: dotnet $($watchArgs -join ' ')" -ForegroundColor Cyan
Write-Host ""

& dotnet $watchArgs
