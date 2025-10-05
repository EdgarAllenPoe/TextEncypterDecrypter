using Moq;
using System.ComponentModel;
using System.Windows.Input;
using TextEncrypterDecrypter.Core.Services;
using TextEncrypterDecrypter.Core.ViewModels;
using TextEncrypterDecrypter.UnitTests.Common;
using Xunit;

namespace TextEncrypterDecrypter.UnitTests.ViewModels;

/// <summary>
/// Unit tests for main view model
/// </summary>
public class MainViewModelTests
{
    private readonly Mock<IEncryptionService> _encryptionServiceMock;
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly Mock<IClipboardService> _clipboardServiceMock;
    private readonly MainViewModel _viewModel;

    public MainViewModelTests()
    {
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _settingsServiceMock = new Mock<ISettingsService>();
        _clipboardServiceMock = new Mock<IClipboardService>();
        _viewModel = new MainViewModel(_encryptionServiceMock.Object, _settingsServiceMock.Object, _clipboardServiceMock.Object);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void EncryptCommand_WhenCanExecute_ExecutesSuccessfully()
    {
        // Arrange
        _viewModel.Text = "Hello, World!";
        _viewModel.Password = "testpassword123";
        _encryptionServiceMock.Setup(x => x.EncryptAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("encrypted_text");

        // Act
        var command = _viewModel.EncryptCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void EncryptCommand_WhenTextIsEmpty_CannotExecute()
    {
        // Arrange
        _viewModel.Text = string.Empty;
        _viewModel.Password = "testpassword123";

        // Act
        var command = _viewModel.EncryptCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void EncryptCommand_WhenPasswordIsEmpty_CannotExecute()
    {
        // Arrange
        _viewModel.Text = "Hello, World!";
        _viewModel.Password = string.Empty;

        // Act
        var command = _viewModel.EncryptCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void DecryptCommand_WhenCanExecute_ExecutesSuccessfully()
    {
        // Arrange
        _viewModel.Text = "encrypted_text";
        _viewModel.Password = "testpassword123";
        _encryptionServiceMock.Setup(x => x.DecryptAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("Hello, World!");

        // Act
        var command = _viewModel.DecryptCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void PropertyChanged_WhenTextChanges_RaisesEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(MainViewModel.Text))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.Text = "New text";

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.Equal("New text", _viewModel.Text);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void PropertyChanged_WhenPasswordChanges_RaisesEvent()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(MainViewModel.Password))
                propertyChangedRaised = true;
        };

        // Act
        _viewModel.Password = "newpassword";

        // Assert
        Assert.True(propertyChangedRaised);
        Assert.Equal("newpassword", _viewModel.Password);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void EncryptAsync_WithValidInput_SetsText()
    {
        // Arrange
        _viewModel.Text = "Hello, World!";
        _viewModel.Password = "testpassword123";
        var expectedEncrypted = "encrypted_result";
        _encryptionServiceMock.Setup(x => x.EncryptAsync("Hello, World!", "testpassword123"))
            .ReturnsAsync(expectedEncrypted);

        // Act
        var command = _viewModel.EncryptCommand;
        command.Execute(null);

        // Assert
        Assert.Equal(expectedEncrypted, _viewModel.Text);
        _encryptionServiceMock.Verify(x => x.EncryptAsync("Hello, World!", "testpassword123"), Times.Once);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void DecryptAsync_WithValidInput_SetsText()
    {
        // Arrange
        _viewModel.Text = "encrypted_text";
        _viewModel.Password = "testpassword123";
        var expectedDecrypted = "Hello, World!";
        _encryptionServiceMock.Setup(x => x.DecryptAsync("encrypted_text", "testpassword123"))
            .ReturnsAsync(expectedDecrypted);

        // Act
        var command = _viewModel.DecryptCommand;
        command.Execute(null);

        // Assert
        Assert.Equal(expectedDecrypted, _viewModel.Text);
        _encryptionServiceMock.Verify(x => x.DecryptAsync("encrypted_text", "testpassword123"), Times.Once);
    }
}
