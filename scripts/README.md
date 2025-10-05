# PowerShell Scripts Guide

This directory contains PowerShell scripts to help with building, testing, and publishing the TextEncrypterDecrypter application. All scripts support modern PowerShell features and include enhanced user experience with emojis, colors, and detailed output.

## 📋 Scripts Overview

| Script | Purpose | Key Features |
|--------|---------|--------------|
| [`build.ps1`](#buildps1) | Build the solution | Clean, restore, verbose output |
| [`test.ps1`](#testps1) | Run tests with coverage | Category filtering, watch mode, coverage reports |
| [`watch-tests.ps1`](#watch-testsp1) | TDD development | Auto-reload on changes, TDD tips |
| [`publish.ps1`](#publishps1) | Publish application | Portable/NativeAOT profiles, file size reporting |

## 🔧 Prerequisites

- **PowerShell 7+** (recommended) or PowerShell 5.1+
- **.NET 8 SDK** installed and available in PATH
- **Windows** (for publishing and running the Avalonia app)

## 📁 Directory Structure

```
TextEncrypterDecrypter/
├── scripts/
│   ├── build.ps1          # Build the solution
│   ├── test.ps1           # Run tests
│   ├── watch-tests.ps1    # TDD watch mode
│   ├── publish.ps1        # Publish application
│   └── README.md          # This file
├── src/                   # Source code
├── tests/                 # Test projects
└── TextEncrypterDecrypter.sln
```

---

## 🔨 build.ps1

Builds the entire TextEncrypterDecrypter solution with appropriate configuration.

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Configuration` | String | `Release` | Build configuration (`Debug` or `Release`) |
| `Clean` | Switch | `False` | Clean before building |
| `Verbose` | Switch | `False` | Enable verbose output |
| `NoRestore` | Switch | `False` | Skip package restore |

### Examples

```powershell
# Basic build in Release configuration
.\build.ps1

# Build in Debug configuration with clean
.\build.ps1 -Configuration Debug -Clean

# Verbose build with no restore (faster after initial restore)
.\build.ps1 -Verbose -NoRestore

# Clean and build in Release with verbose output
.\build.ps1 -Clean -Verbose
```

### Output Example

```
🔨 Building TextEncrypterDecrypter solution...
📦 Configuration: Release
🧹 Cleaning solution...
📥 Restoring packages...
🔨 Building solution...
✅ Build completed successfully!
```

---

## 🧪 test.ps1

Runs all tests with optional coverage collection and reporting.

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Configuration` | String | `Release` | Build configuration (`Debug` or `Release`) |
| `Category` | String | - | Test category filter (`unit`, `integration`, `ui`, `slow`, `external`) |
| `Coverage` | Switch | `False` | Generate coverage report |
| `Watch` | Switch | `False` | Run tests in watch mode |
| `Verbose` | Switch | `False` | Enable verbose output |
| `NoBuild` | Switch | `False` | Skip build step |

### Examples

```powershell
# Run all tests
.\test.ps1

# Run only unit tests with coverage
.\test.ps1 -Category unit -Coverage

# Run tests in watch mode
.\test.ps1 -Watch

# Run integration tests in Debug configuration
.\test.ps1 -Category integration -Configuration Debug

# Run UI tests with verbose output
.\test.ps1 -Category ui -Verbose
```

### Coverage Reports

When using the `-Coverage` parameter, the script will:

1. Collect code coverage data during test execution
2. Generate HTML and Cobertura reports in `./coverage/report/`
3. Install `reportgenerator-globaltool` if not already available
4. Display the path to the HTML report: `coverage/report/index.html`

### Output Example

```
🧪 Running TextEncrypterDecrypter tests...
📦 Configuration: Release
🏷️  Category: unit
📊 Coverage collection enabled
🚀 Executing: dotnet test --configuration Release --no-build --collect:XPlat Code Coverage --results-directory ./coverage --filter Category=unit

Test Run Successful.
Total tests: 44
     Passed: 44

📊 Generating coverage report...
📊 Coverage report generated at: coverage/report/index.html
✅ Tests completed successfully!
```

---

## 👀 watch-tests.ps1

Perfect for Test-Driven Development (TDD). Automatically re-runs tests when source files change.

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Category` | String | - | Test category filter (`unit`, `integration`, `ui`, `slow`, `external`) |
| `Configuration` | String | `Debug` | Build configuration (`Debug` or `Release`) |
| `Verbose` | Switch | `False` | Enable verbose output |
| `Project` | String | - | Specific project to watch (relative path to .csproj file) |

### Examples

```powershell
# Watch all tests (TDD mode)
.\watch-tests.ps1

# Watch only unit tests
.\watch-tests.ps1 -Category unit

# Watch specific test project
.\watch-tests.ps1 -Project tests/TextEncrypterDecrypter.UnitTests/TextEncrypterDecrypter.UnitTests.csproj

# Watch with verbose output
.\watch-tests.ps1 -Verbose
```

### TDD Workflow

The script displays TDD tips and follows the Red-Green-Refactor cycle:

1. **🔴 RED**: Write a failing test first
2. **🟢 GREEN**: Make it pass with minimal code
3. **🔵 REFACTOR**: Refactor while keeping tests green

### Output Example

```
👀 Starting test watch mode for TextEncrypterDecrypter...
📦 Configuration: Debug
🏷️  Category: unit
👁️  Watching for changes in unit tests...

💡 TDD Tips:
   • Write a failing test first (RED)
   • Make it pass with minimal code (GREEN)
   • Refactor while keeping tests green (REFACTOR)

Press Ctrl+C to stop watching

🚀 Executing: dotnet watch test --configuration Debug --filter Category=unit
```

---

## 📦 publish.ps1

Publishes the application using predefined publish profiles with enhanced output and file size reporting.

### Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Profile` | String | `Portable` | Publish profile (`Portable` or `NativeAot`) |
| `Configuration` | String | `Release` | Build configuration (`Debug` or `Release`) |
| `Open` | Switch | `False` | Open the publish directory after publishing |
| `Verbose` | Switch | `False` | Enable verbose output |
| `Clean` | Switch | `False` | Clean before publishing |

### Publish Profiles

| Profile | Description | Output |
|---------|-------------|---------|
| `Portable` | Self-contained, single-file executable | `TextEncrypterDecrypter.App.exe` |
| `NativeAot` | Native Ahead-of-Time compiled executable | `TextEncrypterDecrypter.App.exe` (smaller, faster startup) |

### Examples

```powershell
# Basic publish using Portable profile
.\publish.ps1

# Publish using NativeAOT profile
.\publish.ps1 -Profile NativeAot

# Publish and open output directory
.\publish.ps1 -Open

# Clean publish with verbose output
.\publish.ps1 -Clean -Verbose

# Debug publish with NativeAOT
.\publish.ps1 -Profile NativeAot -Configuration Debug
```

### Output Example

```
📦 Publishing TextEncrypterDecrypter application...
🔧 Profile: Portable
📦 Configuration: Release
🔨 Building solution...
📦 Publishing application...

✅ Publish completed successfully!
📁 Output directory: src/TextEncrypterDecrypter.App/bin/Release/net8.0/publish/Portable

📋 Publish directory contents:
   📄 TextEncrypterDecrypter.App.exe (15.2 MB)
   📄 TextEncrypterDecrypter.App.pdb (2.1 MB)
   📄 TextEncrypterDecrypter.Core.dll (8.5 KB)

📊 Total size: 17.3 MB
✅ Publish completed!
```

---

## 🚀 Common Workflows

### Development Workflow

```powershell
# 1. Build the solution
.\build.ps1 -Configuration Debug

# 2. Run tests to ensure everything works
.\test.ps1 -Configuration Debug

# 3. Start TDD mode for feature development
.\watch-tests.ps1 -Category unit
```

### CI/CD Workflow

```powershell
# 1. Clean build
.\build.ps1 -Clean -Configuration Release

# 2. Run all tests with coverage
.\test.ps1 -Configuration Release -Coverage

# 3. Publish the application
.\publish.ps1 -Configuration Release -Profile Portable
```

### Quick Testing Workflow

```powershell
# Run only unit tests quickly
.\test.ps1 -Category unit -NoBuild

# Watch integration tests
.\watch-tests.ps1 -Category integration
```

---

## 🛠️ Troubleshooting

### Common Issues

**❌ "Solution file not found"**
- Ensure you're running scripts from the solution root directory
- Check that `TextEncrypterDecrypter.sln` exists

**❌ "Build failed"**
- Run `dotnet restore` manually first
- Check for compilation errors in the output
- Try cleaning first: `.\build.ps1 -Clean`

**❌ "Tests failed"**
- Check the test output for specific failure details
- Run with verbose output: `.\test.ps1 -Verbose`
- Try running individual test categories

**❌ "Publish failed"**
- Ensure the build succeeds first: `.\build.ps1`
- Check publish profile configuration
- Try cleaning first: `.\publish.ps1 -Clean`

### Performance Tips

- Use `-NoBuild` or `-NoRestore` for faster iterations
- Use `Debug` configuration for faster builds during development
- Use `Release` configuration for final testing and publishing
- Use specific test categories to run only relevant tests

---

## 📚 Additional Resources

- [.NET CLI Documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/)
- [xUnit Testing Framework](https://xunit.net/)
- [PowerShell Documentation](https://docs.microsoft.com/en-us/powershell/)
- [Avalonia UI Framework](https://avaloniaui.net/)

---

## 🤝 Contributing

When adding new scripts or modifying existing ones:

1. Follow the established parameter naming conventions
2. Include comprehensive help documentation
3. Add emojis and colors for better UX
4. Test scripts in both PowerShell 5.1 and PowerShell 7+
5. Update this README with any new features or parameters

---

*Last updated: $(Get-Date -Format "yyyy-MM-dd")*
