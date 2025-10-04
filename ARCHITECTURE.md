# Architecture Guidelines

## Project Boundaries

### TextEncrypterDecrypter.Core
- **Purpose**: Contains all business logic, models, services, and view models
- **Dependencies**: No UI dependencies allowed (no Avalonia, WPF, etc.)
- **Contains**:
  - Models (data structures)
  - Services (business logic, encryption/decryption)
  - ViewModels (MVVM pattern, no UI dependencies)
  - Interfaces (abstractions for dependency injection)
  - Utilities (helper classes)

### TextEncrypterDecrypter.App
- **Purpose**: UI layer and application composition root
- **Dependencies**: Only references Core project
- **Contains**:
  - Views (Avalonia XAML files)
  - Application startup and composition
  - Dependency injection configuration
  - Theming and styling
  - Platform-specific implementations

### Test Projects
- **TextEncrypterDecrypter.UnitTests**: Tests for Core project only (pure logic)
- **TextEncrypterDecrypter.IntegrationTests**: Tests for I/O operations, DI, file operations
- **TextEncrypterDecrypter.UITests**: Headless UI tests with Avalonia.Headless

## MVVM Guidelines

### ViewModels (in Core)
- Implement `INotifyPropertyChanged` or use `ObservableObject` base class
- No direct UI references (no Avalonia types)
- Use dependency injection for services
- Handle business logic and state management
- Expose commands using `ICommand` implementations

### Views (in App)
- Pure XAML with data binding to ViewModels
- No business logic in code-behind
- Use compiled bindings for better performance
- Minimal code-behind (only UI-specific logic)

### Services
- Interface-based design for testability
- Single responsibility principle
- Stateless where possible
- Use dependency injection

## Settings and Logging

### Settings Location
- **Portable**: Store in application directory alongside executable
- **User Settings**: Use `Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)`
- **Format**: JSON files for simplicity and portability

### Logging
- **Location**: Application data folder
- **Framework**: Microsoft.Extensions.Logging
- **Levels**: Debug, Information, Warning, Error
- **Rotation**: Daily log files with size limits

### File Structure
```
TextEncrypterDecrypter.exe (portable)
├── settings.json (application settings)
├── logs/
│   ├── app-2024-01-01.log
│   └── app-2024-01-02.log
└── temp/ (temporary files)
```

## Dependency Injection

### Container
- Use Microsoft.Extensions.DependencyInjection
- Configure in App.xaml.cs or Program.cs
- Register services by interface

### Service Lifetime
- **Singleton**: Application-wide services (settings, encryption)
- **Scoped**: Per-operation services
- **Transient**: Stateless services

## Testing Strategy

### Unit Tests
- Test individual classes and methods
- Mock external dependencies
- Fast execution (< 100ms per test)
- High coverage of business logic

### Integration Tests
- Test service interactions
- File system operations
- JSON serialization/deserialization
- Dependency injection scenarios

### UI Tests
- Headless rendering with Avalonia.Headless
- Visual regression testing
- User interaction simulation
- Window state validation

## Security Considerations

### Encryption
- Use .NET built-in cryptography APIs
- Secure password handling (no plain text storage)
- Salt and hash passwords appropriately
- Use secure random number generation

### File Handling
- Validate file paths to prevent directory traversal
- Sanitize user inputs
- Handle file permissions gracefully
- Secure temporary file cleanup

## Performance Guidelines

### Memory Management
- Implement IDisposable where appropriate
- Use object pooling for frequently created objects
- Monitor memory usage in long-running operations

### UI Responsiveness
- Use async/await for I/O operations
- Implement cancellation tokens
- Avoid blocking UI thread
- Use background threads for heavy operations
