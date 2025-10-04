# TextEncrypterDecrypter

A portable desktop application for encrypting and decrypting text using user-provided passwords.

## Features

- Text encryption and decryption using AES encryption
- Password-based security with PBKDF2 key derivation
- Portable desktop application built with .NET 8 and Avalonia UI
- Cross-platform support (Windows, Linux, macOS)
- Settings persistence
- Self-contained single-file deployment

## Development

This project follows Test-Driven Development (TDD) practices with comprehensive testing coverage.

### Prerequisites

- .NET 8 SDK
- Git
- PowerShell (for scripts)

### Getting Started

1. Clone the repository
2. Run `dotnet restore` to restore packages
3. Run `dotnet build` to build the solution
4. Run `dotnet test` to run all tests

### Scripts

- `scripts/build.ps1` - Build the solution
- `scripts/test.ps1` - Run all tests with coverage
- `scripts/watch-tests.ps1` - Run tests in watch mode for TDD
- `scripts/publish.ps1` - Publish the application

### TDD Development

This project is set up for Test-Driven Development:

1. **Unit Tests** - Fast tests for business logic (Category: unit)
2. **Integration Tests** - Tests for service interactions (Category: integration)  
3. **UI Tests** - Headless UI tests with Avalonia.Headless (Category: ui)

Run specific test categories:
```bash
dotnet test --filter "Category=unit"
dotnet test --filter "Category=integration"
dotnet test --filter "Category=ui"
```

## Architecture

- **TextEncrypterDecrypter.Core** - Core business logic, models, services, and view models (no UI dependencies)
- **TextEncrypterDecrypter.App** - Avalonia UI application and composition root
- **TextEncrypterDecrypter.UnitTests** - Unit tests for core logic
- **TextEncrypterDecrypter.IntegrationTests** - Integration tests for file operations and DI
- **TextEncrypterDecrypter.UITests** - UI tests with headless Avalonia rendering

## Project Structure

```
├── src/
│   ├── TextEncrypterDecrypter.Core/          # Business logic, services, view models
│   └── TextEncrypterDecrypter.App/           # UI layer, Avalonia application
├── tests/
│   ├── TextEncrypterDecrypter.UnitTests/     # Unit tests
│   ├── TextEncrypterDecrypter.IntegrationTests/  # Integration tests
│   └── TextEncrypterDecrypter.UITests/       # UI tests
├── scripts/                                  # PowerShell scripts for development
├── .github/workflows/                        # CI/CD pipeline
└── docs/                                     # Architecture and planning docs
```

## Publishing

The application supports two publish profiles:

### Portable (Default)
```bash
.\scripts\publish.ps1 -Profile Portable
```
- Self-contained executable
- Single file deployment
- No trimming (larger but more compatible)

### NativeAOT
```bash
.\scripts\publish.ps1 -Profile NativeAot
```
- Ahead-of-time compilation
- Smaller executable size
- Faster startup time

## CI/CD

The project includes a GitHub Actions workflow that:
- Builds the solution on Windows
- Runs all tests with coverage collection
- Publishes the portable application
- Uploads artifacts and coverage reports

## Development Guidelines

See [ARCHITECTURE.md](ARCHITECTURE.md) for detailed architectural guidelines.

See [TDD_PLAN.md](TDD_PLAN.md) for the test-driven development plan.

See [NEXT_STEPS.md](NEXT_STEPS.md) for the next implementation steps.

## License

[Add your license here]
