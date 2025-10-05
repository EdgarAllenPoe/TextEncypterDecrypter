#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Runs tests for the TextEncrypterDecrypter solution

.DESCRIPTION
    This script runs all tests with coverage collection and reporting.
    Supports filtering by category and configuration.

.PARAMETER Configuration
    Build configuration (Debug or Release). Defaults to Release.

.PARAMETER Category
    Test category filter (unit, integration, ui, slow, external).

.PARAMETER Coverage
    Generate coverage report.

.PARAMETER Watch
    Run tests in watch mode.

.PARAMETER Verbose
    Enable verbose output.

.PARAMETER NoBuild
    Skip build step.

.EXAMPLE
    .\test.ps1
    Runs all tests in Release configuration.

.EXAMPLE
    .\test.ps1 -Category unit -Coverage
    Runs only unit tests with coverage report.

.EXAMPLE
    .\test.ps1 -Watch
    Runs tests in watch mode.
#>

param(
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [ValidateSet("unit", "integration", "ui", "slow", "external")]
    [string]$Category,
    
    [switch]$Coverage,
    [switch]$Watch,
    [switch]$Verbose,
    [switch]$NoBuild
)

$ErrorActionPreference = "Stop"

# Ensure we're in the correct directory
if (-not (Test-Path "TextEncrypterDecrypter.sln")) {
    Write-Error "Solution file not found. Please run this script from the solution root directory."
    exit 1
}

Write-Host "🧪 Running TextEncrypterDecrypter tests..." -ForegroundColor Green
Write-Host "📦 Configuration: $Configuration" -ForegroundColor Yellow

if ($Category) {
    Write-Host "🏷️  Category: $Category" -ForegroundColor Yellow
}

if ($Coverage) {
    Write-Host "📊 Coverage collection enabled" -ForegroundColor Yellow
}

if ($Watch) {
    Write-Host "👀 Watch mode enabled" -ForegroundColor Yellow
}

$testArgs = @("test", "--configuration", $Configuration)

if (-not $NoBuild) {
    $testArgs += "--no-build"
}

if ($Coverage) {
    $testArgs += @("--collect:XPlat Code Coverage", "--results-directory", "./coverage")
}

if ($Category) {
    $testArgs += @("--filter", "Category=$Category")
}

if ($Verbose) {
    $testArgs += @("--verbosity", "normal")
}

if ($Watch) {
    $testArgs = @("watch", "test") + $testArgs[2..($testArgs.Length-1)]
}

Write-Host "🚀 Executing: dotnet $($testArgs -join ' ')" -ForegroundColor Cyan
Write-Host ""

& dotnet $testArgs

if ($LASTEXITCODE -ne 0) {
    Write-Error "❌ Tests failed"
    exit 1
}

if ($Coverage -and -not $Watch) {
    Write-Host ""
    Write-Host "📊 Generating coverage report..." -ForegroundColor Yellow
    
    # Install reportgenerator if not already installed
    $reportGenerator = dotnet tool list -g | Select-String "reportgenerator-globaltool"
    if (-not $reportGenerator) {
        Write-Host "📦 Installing reportgenerator-globaltool..." -ForegroundColor Yellow
        dotnet tool install -g dotnet-reportgenerator-globaltool
    }
    
    # Generate HTML report
    reportgenerator -reports:"coverage/**/*.cobertura.xml" -targetdir:"coverage/report" -reporttypes:"Html;Cobertura"
    
    Write-Host "📊 Coverage report generated at: coverage/report/index.html" -ForegroundColor Green
}

Write-Host ""
Write-Host "✅ Tests completed successfully!" -ForegroundColor Green
