# TDD Implementation Plan

## First Slice: Basic Text Encryption/Decryption

### User Story
As a user, I want to encrypt and decrypt text using a password so that I can securely store and retrieve sensitive information.

### Acceptance Criteria
1. **Given** a user enters text and a password, **when** they click encrypt, **then** they should see encrypted text
2. **Given** a user has encrypted text and the correct password, **when** they click decrypt, **then** they should see the original text
3. **Given** a user has encrypted text and an incorrect password, **when** they click decrypt, **then** they should see an error message
4. **Given** a user has encrypted text, **when** they restart the application, **then** the encrypted text should persist
5. **Given** a user enters empty text or empty password, **when** they try to encrypt, **then** they should see validation errors

## Test Pyramid

### Unit Tests (70% of tests)
**Target**: Core business logic and ViewModels

#### EncryptionService Tests
- `EncryptText_WithValidInput_ReturnsEncryptedText`
- `DecryptText_WithValidInput_ReturnsOriginalText`
- `DecryptText_WithInvalidPassword_ThrowsSecurityException`
- `EncryptText_WithEmptyText_ThrowsArgumentException`
- `EncryptText_WithEmptyPassword_ThrowsArgumentException`

#### MainViewModel Tests
- `EncryptCommand_WhenCanExecute_ExecutesSuccessfully`
- `EncryptCommand_WithEmptyText_CannotExecute`
- `DecryptCommand_WhenCanExecute_ExecutesSuccessfully`
- `DecryptCommand_WithEmptyText_CannotExecute`
- `PropertyChanged_WhenTextChanges_RaisesEvent`
- `PropertyChanged_WhenPasswordChanges_RaisesEvent`

#### AppSettings Tests
- `LoadSettings_WithValidFile_ReturnsSettings`
- `SaveSettings_WithValidSettings_SavesToFile`
- `LoadSettings_WithInvalidFile_ReturnsDefaultSettings`

### Integration Tests (20% of tests)
**Target**: Service interactions and file operations

#### SettingsService Integration Tests
- `SettingsService_CanSaveAndLoadSettings_RoundTripSuccess`
- `SettingsService_WithCorruptedFile_HandlesGracefully`
- `SettingsService_WithMissingFile_CreatesDefault`

#### EncryptionService Integration Tests
- `EncryptionService_WithRealCryptoProvider_EncryptsAndDecrypts`
- `EncryptionService_WithDifferentPasswords_ProducesDifferentResults`

### UI Tests (10% of tests)
**Target**: End-to-end user interactions

#### MainWindow UI Tests
- `MainWindow_CanBeRendered_WithoutErrors`
- `MainWindow_WithValidInput_ShowsEncryptedText`
- `MainWindow_WithInvalidPassword_ShowsError`
- `MainWindow_CanLoadAndSaveSettings_PersistsData`

## Test Naming Conventions

### Format
`MethodName_StateUnderTest_ExpectedBehavior`

### Examples
- `EncryptText_WithValidInput_ReturnsEncryptedText`
- `DecryptText_WithInvalidPassword_ThrowsSecurityException`
- `PropertyChanged_WhenTextChanges_RaisesEvent`

## Test Organization

### File Structure
```
tests/
├── TextEncrypterDecrypter.UnitTests/
│   ├── Services/
│   │   ├── EncryptionServiceTests.cs
│   │   └── SettingsServiceTests.cs
│   ├── ViewModels/
│   │   └── MainViewModelTests.cs
│   └── Models/
│       └── AppSettingsTests.cs
├── TextEncrypterDecrypter.IntegrationTests/
│   ├── Services/
│   │   ├── EncryptionServiceIntegrationTests.cs
│   │   └── SettingsServiceIntegrationTests.cs
└── TextEncrypterDecrypter.UITests/
    └── Views/
        └── MainWindowTests.cs
```

### Test Categories
- `[Trait("Category", "unit")]` - Fast unit tests
- `[Trait("Category", "integration")]` - Integration tests
- `[Trait("Category", "ui")]` - UI tests
- `[Trait("Category", "slow")]` - Slow tests
- `[Trait("Category", "external")]` - Tests requiring external dependencies

## Implementation Order

### Phase 1: Core Services
1. Create `IEncryptionService` interface
2. Implement `EncryptionService` with unit tests
3. Create `ISettingsService` interface
4. Implement `SettingsService` with unit tests

### Phase 2: Models and ViewModels
1. Create `AppSettings` model with tests
2. Create `MainViewModel` with unit tests
3. Implement commands and property change notifications

### Phase 3: Integration
1. Create integration tests for services
2. Test file operations and persistence
3. Test service interactions

### Phase 4: UI
1. Create basic MainWindow XAML
2. Implement data binding
3. Create UI tests with Avalonia.Headless

### Phase 5: Polish
1. Add error handling and validation
2. Implement settings persistence
3. Add logging and diagnostics

## Success Criteria

### Coverage Targets
- **Overall**: 80% line coverage
- **Core Services**: 95% coverage
- **ViewModels**: 90% coverage
- **Integration**: 70% coverage

### Performance Targets
- **Unit Tests**: < 100ms per test
- **Integration Tests**: < 1s per test
- **UI Tests**: < 5s per test
- **Total Test Suite**: < 30s

### Quality Gates
- All tests must pass
- No warnings in build
- Coverage threshold met
- Static analysis clean
- UI tests render without errors
