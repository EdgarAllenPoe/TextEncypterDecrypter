# TDD Red/Green/Refactor Cycle Setup

## First Red/Green Cycle Sequence

### Step 1: Write Failing Unit Tests (RED)
1. **Remove `[Skip]` attribute** from placeholder tests
2. **Run tests** - should fail with "Test not implemented"
3. **Verify red state** - all tests fail as expected

### Step 2: Implement Minimal Code (GREEN)
1. **Create interfaces** first:
   - `IEncryptionService` with `EncryptAsync` and `DecryptAsync` methods
   - `ISettingsService` with `LoadAsync` and `SaveAsync` methods

2. **Implement core models**:
   - `AppSettings` with basic properties
   - `EncryptionResult` with success/failure states

3. **Implement services** with minimal functionality:
   - `EncryptionService` with basic AES encryption
   - `JsonSettingsService` with file I/O

4. **Implement MainViewModel** with:
   - Properties for Text, Password, EncryptedText
   - Commands for Encrypt and Decrypt
   - Basic property change notifications

5. **Run tests** - should pass (GREEN)

### Step 3: Refactor (REFACTOR)
1. **Extract common patterns**
2. **Improve error handling**
3. **Add validation**
4. **Optimize performance**
5. **Run tests** - should still pass (GREEN)

## Detailed Implementation Order

### Phase 1: Core Interfaces and Models
```csharp
// 1. Create IEncryptionService
public interface IEncryptionService
{
    Task<string> EncryptAsync(string text, string password);
    Task<string> DecryptAsync(string encryptedText, string password);
}

// 2. Create ISettingsService  
public interface ISettingsService
{
    Task<AppSettings> LoadAsync();
    Task SaveAsync(AppSettings settings);
}

// 3. Create AppSettings model
public class AppSettings
{
    public string? LastUsedPassword { get; set; }
    public string? LastEncryptedText { get; set; }
    public DateTime LastUsed { get; set; }
}
```

### Phase 2: Service Implementations
```csharp
// 4. Implement EncryptionService
public class EncryptionService : IEncryptionService
{
    public async Task<string> EncryptAsync(string text, string password)
    {
        // Basic AES encryption implementation
        throw new NotImplementedException();
    }
    
    public async Task<string> DecryptAsync(string encryptedText, string password)
    {
        // Basic AES decryption implementation
        throw new NotImplementedException();
    }
}

// 5. Implement JsonSettingsService
public class JsonSettingsService : ISettingsService
{
    public async Task<AppSettings> LoadAsync()
    {
        // JSON file loading implementation
        throw new NotImplementedException();
    }
    
    public async Task SaveAsync(AppSettings settings)
    {
        // JSON file saving implementation
        throw new NotImplementedException();
    }
}
```

### Phase 3: ViewModel Implementation
```csharp
// 6. Implement MainViewModel
public class MainViewModel : INotifyPropertyChanged
{
    private string _text = string.Empty;
    private string _password = string.Empty;
    private string _encryptedText = string.Empty;
    
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }
    
    public ICommand EncryptCommand { get; }
    public ICommand DecryptCommand { get; }
    
    // TODO: Implement property change notifications and commands
}
```

## Test Implementation Sequence

### Unit Tests (Start Here)
1. **EncryptionServiceTests**:
   - `EncryptText_WithValidInput_ReturnsEncryptedText`
   - `DecryptText_WithValidInput_ReturnsOriginalText`
   - `DecryptText_WithInvalidPassword_ThrowsSecurityException`

2. **MainViewModelTests**:
   - `EncryptCommand_WhenCanExecute_ExecutesSuccessfully`
   - `PropertyChanged_WhenTextChanges_RaisesEvent`

### Integration Tests (After Unit Tests Pass)
3. **SettingsServiceIntegrationTests**:
   - `SettingsService_CanSaveAndLoadSettings_RoundTripSuccess`

### UI Tests (After Integration Tests Pass)
4. **MainWindowTests**:
   - `MainWindow_CanBeRendered_WithoutErrors`

## Coverage Thresholds

### Local Development
- **Minimum**: 80% line coverage
- **Target**: 90% line coverage
- **Enforcement**: Fail build if below 80%

### CI Pipeline
- **Minimum**: 85% line coverage
- **Target**: 95% line coverage
- **Enforcement**: Fail CI if below 85%

## Test Categories and Filters

### Fast Tests (Run Frequently)
```bash
dotnet test --filter "Category=unit"
```

### All Tests (Run Before Commit)
```bash
dotnet test
```

### Slow Tests (Run Before Release)
```bash
dotnet test --filter "Category=integration|Category=ui"
```

## Watch Mode Commands

### Watch Unit Tests
```bash
dotnet watch test --filter "Category=unit"
```

### Watch All Tests
```bash
dotnet watch test
```

## Red/Green/Refactor Checklist

### Before Starting (RED)
- [ ] All tests fail
- [ ] No implementation exists
- [ ] Test names describe expected behavior
- [ ] Test coverage target defined

### Implementation (GREEN)
- [ ] Write minimal code to pass tests
- [ ] All tests pass
- [ ] No unnecessary code
- [ ] Code is clean and readable

### Refactoring (REFACTOR)
- [ ] All tests still pass
- [ ] Code is DRY (Don't Repeat Yourself)
- [ ] Code follows SOLID principles
- [ ] Performance is acceptable
- [ ] Error handling is robust

## Success Metrics

### Code Quality
- All tests pass
- No compiler warnings
- Static analysis clean
- Coverage threshold met

### Performance
- Unit tests: < 100ms each
- Integration tests: < 1s each
- UI tests: < 5s each
- Total test suite: < 30s

### Maintainability
- Clear test names
- Single responsibility per test
- Minimal test dependencies
- Easy to understand failures
