# Next Steps for First Feature Implementation

## Immediate Actions

### 1. Enable First Unit Tests (RED Phase)
Remove the `Assert.Fail("Test not implemented");` lines and start implementing the actual test logic:

#### Priority Order:
1. **EncryptionServiceTests** - Core business logic
2. **MainViewModelTests** - UI logic and commands
3. **SettingsServiceIntegrationTests** - File operations
4. **MainWindowTests** - UI rendering

### 2. Implement Core Services (GREEN Phase)

#### Step 1: Create IEncryptionService Interface
```csharp
// src/TextEncrypterDecrypter.Core/Services/IEncryptionService.cs
public interface IEncryptionService
{
    Task<string> EncryptAsync(string text, string password);
    Task<string> DecryptAsync(string encryptedText, string password);
}
```

#### Step 2: Implement EncryptionService
```csharp
// src/TextEncrypterDecrypter.Core/Services/EncryptionService.cs
public class EncryptionService : IEncryptionService
{
    public async Task<string> EncryptAsync(string text, string password)
    {
        // Use AES encryption with PBKDF2 key derivation
        // Return base64-encoded encrypted text
    }
    
    public async Task<string> DecryptAsync(string encryptedText, string password)
    {
        // Decode base64 and decrypt using AES
        // Return original text
    }
}
```

#### Step 3: Create AppSettings Model
```csharp
// src/TextEncrypterDecrypter.Core/Models/AppSettings.cs
public class AppSettings
{
    public string? LastUsedPassword { get; set; }
    public string? LastEncryptedText { get; set; }
    public DateTime LastUsed { get; set; } = DateTime.Now;
    public bool RememberPassword { get; set; } = false;
}
```

#### Step 4: Implement JsonSettingsService
```csharp
// src/TextEncrypterDecrypter.Core/Services/JsonSettingsService.cs
public class JsonSettingsService : ISettingsService
{
    private readonly string _settingsPath;
    
    public async Task<AppSettings> LoadAsync()
    {
        // Load from JSON file or return defaults
    }
    
    public async Task SaveAsync(AppSettings settings)
    {
        // Save to JSON file
    }
}
```

#### Step 5: Implement MainViewModel
```csharp
// src/TextEncrypterDecrypter.Core/ViewModels/MainViewModel.cs
public class MainViewModel : INotifyPropertyChanged
{
    private readonly IEncryptionService _encryptionService;
    private readonly ISettingsService _settingsService;
    
    private string _text = string.Empty;
    private string _password = string.Empty;
    private string _encryptedText = string.Empty;
    private bool _isLoading = false;
    
    public string Text
    {
        get => _text;
        set => SetProperty(ref _text, value);
    }
    
    public ICommand EncryptCommand { get; }
    public ICommand DecryptCommand { get; }
    
    // Property change notifications and command implementations
}
```

### 3. Update App Layer (GREEN Phase)

#### Step 1: Update MainWindow XAML
```xml
<!-- src/TextEncrypterDecrypter.App/Views/MainWindow.axaml -->
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vm="using:TextEncrypterDecrypter.Core.ViewModels"
        x:DataType="vm:MainViewModel"
        x:Class="TextEncrypterDecrypter.App.Views.MainWindow"
        Title="TextEncrypterDecrypter" Width="600" Height="400">

  <Grid Margin="20">
    <Grid.RowDefinitions>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="Auto"/>
      <RowDefinition Height="*"/>
      <RowDefinition Height="Auto"/>
    </Grid.RowDefinitions>
    
    <!-- Text Input -->
    <TextBox Grid.Row="0" 
             Text="{Binding Text}" 
             Watermark="Enter text to encrypt/decrypt..."
             Margin="0,0,0,10"/>
    
    <!-- Password Input -->
    <TextBox Grid.Row="1" 
             Text="{Binding Password}" 
             PasswordChar="*"
             Watermark="Enter password..."
             Margin="0,0,0,10"/>
    
    <!-- Buttons -->
    <StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,0,0,10">
      <Button Content="Encrypt" 
              Command="{Binding EncryptCommand}"
              Margin="0,0,10,0"/>
      <Button Content="Decrypt" 
              Command="{Binding DecryptCommand}"/>
    </StackPanel>
    
    <!-- Encrypted Text Output -->
    <TextBox Grid.Row="3" 
             Text="{Binding EncryptedText}" 
             IsReadOnly="True"
             Watermark="Encrypted text will appear here..."
             Margin="0,0,0,10"/>
    
    <!-- Status/Error Messages -->
    <TextBlock Grid.Row="5" 
               Text="{Binding StatusMessage}"
               Foreground="Red"/>
  </Grid>
</Window>
```

#### Step 2: Configure Dependency Injection
```csharp
// src/TextEncrypterDecrypter.App/Program.cs
public static void Main(string[] args)
{
    var builder = AvaloniaAppBuilder.Configure<App>()
        .UsePlatformDetect()
        .WithInterFont()
        .LogToTrace();

    // Configure services
    builder.Services.AddSingleton<IEncryptionService, EncryptionService>();
    builder.Services.AddSingleton<ISettingsService, JsonSettingsService>();
    builder.Services.AddTransient<MainViewModel>();

    builder.StartWithClassicDesktopLifetime(args);
}
```

### 4. Integration Tests (GREEN Phase)

#### SettingsService Integration Tests
```csharp
[Fact]
[Trait("Category", TestCategories.Integration)]
public async Task SettingsService_CanSaveAndLoadSettings_RoundTripSuccess()
{
    // Arrange
    var settingsService = new JsonSettingsService();
    var originalSettings = new AppSettings
    {
        LastUsedPassword = "test123",
        LastEncryptedText = "encrypted_data",
        LastUsed = DateTime.Now,
        RememberPassword = true
    };
    
    // Act
    await settingsService.SaveAsync(originalSettings);
    var loadedSettings = await settingsService.LoadAsync();
    
    // Assert
    Assert.Equal(originalSettings.LastUsedPassword, loadedSettings.LastUsedPassword);
    Assert.Equal(originalSettings.LastEncryptedText, loadedSettings.LastEncryptedText);
    Assert.Equal(originalSettings.RememberPassword, loadedSettings.RememberPassword);
}
```

### 5. UI Tests (GREEN Phase)

#### MainWindow UI Tests
```csharp
[Fact]
[Trait("Category", TestCategories.UI)]
public void MainWindow_CanBeRendered_WithoutErrors()
{
    // Arrange
    var app = AvaloniaApp.GetApp();
    var window = new MainWindow();
    
    // Act & Assert
    Assert.NotNull(window);
    // Additional UI validation can be added here
}
```

## Development Workflow

### 1. TDD Cycle
1. **Write failing test** (RED)
2. **Implement minimal code** to pass (GREEN)
3. **Refactor** while keeping tests green (REFACTOR)

### 2. Test Commands
```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test --filter "Category=unit"

# Run tests in watch mode
dotnet watch test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### 3. Build and Publish
```bash
# Build solution
.\scripts\build.ps1

# Publish portable app
.\scripts\publish.ps1

# Publish NativeAOT app
.\scripts\publish.ps1 -Profile NativeAot
```

## Success Criteria

### Phase 1 Complete When:
- [ ] All unit tests pass
- [ ] Basic encryption/decryption works
- [ ] Settings can be saved and loaded
- [ ] MainWindow renders without errors

### Phase 2 Complete When:
- [ ] Integration tests pass
- [ ] UI tests pass
- [ ] Application can be published
- [ ] End-to-end encryption/decryption works

### Phase 3 Complete When:
- [ ] Error handling is robust
- [ ] User experience is smooth
- [ ] Performance is acceptable
- [ ] Code coverage meets targets (80%+)

## File Structure After Implementation

```
src/TextEncrypterDecrypter.Core/
├── Models/
│   └── AppSettings.cs
├── Services/
│   ├── IEncryptionService.cs
│   ├── EncryptionService.cs
│   ├── ISettingsService.cs
│   └── JsonSettingsService.cs
└── ViewModels/
    └── MainViewModel.cs

src/TextEncrypterDecrypter.App/
├── Views/
│   ├── MainWindow.axaml
│   └── MainWindow.axaml.cs
├── Program.cs
└── App.axaml.cs

tests/
├── TextEncrypterDecrypter.UnitTests/
│   ├── Services/EncryptionServiceTests.cs
│   └── ViewModels/MainViewModelTests.cs
├── TextEncrypterDecrypter.IntegrationTests/
│   └── Services/SettingsServiceIntegrationTests.cs
└── TextEncrypterDecrypter.UITests/
    └── Views/MainWindowTests.cs
```

## Next Development Session

1. Start with `EncryptionServiceTests.EncryptText_WithValidInput_ReturnsEncryptedText`
2. Implement `IEncryptionService` interface
3. Implement basic `EncryptionService` class
4. Run tests to verify GREEN state
5. Refactor if needed
6. Move to next test

This provides a clear roadmap for implementing the first feature using TDD principles.
