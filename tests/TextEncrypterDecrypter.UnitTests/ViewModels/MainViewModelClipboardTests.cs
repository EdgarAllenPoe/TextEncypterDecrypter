using Moq;
using System.Windows.Input;
using TextEncrypterDecrypter.Core.Services;
using TextEncrypterDecrypter.Core.ViewModels;
using TextEncrypterDecrypter.UnitTests.Common;
using Xunit;

namespace TextEncrypterDecrypter.UnitTests.ViewModels;

/// <summary>
/// Unit tests for clipboard functionality in MainViewModel
/// </summary>
public class MainViewModelClipboardTests
{
    private readonly Mock<IEncryptionService> _encryptionServiceMock;
    private readonly Mock<ISettingsService> _settingsServiceMock;
    private readonly Mock<IClipboardService> _clipboardServiceMock;
    private readonly MainViewModel _viewModel;

    public MainViewModelClipboardTests()
    {
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _settingsServiceMock = new Mock<ISettingsService>();
        _clipboardServiceMock = new Mock<IClipboardService>();
        _viewModel = new MainViewModel(_encryptionServiceMock.Object, _settingsServiceMock.Object, _clipboardServiceMock.Object);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void CopyTextCommand_WhenTextExists_CanExecute()
    {
        // Arrange
        _viewModel.Text = "encrypted_text_here";

        // Act
        var command = _viewModel.CopyTextCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void CopyTextCommand_WhenTextIsEmpty_CannotExecute()
    {
        // Arrange
        _viewModel.Text = string.Empty;

        // Act
        var command = _viewModel.CopyTextCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void CopyTextCommand_WhenLoading_CannotExecute()
    {
        // Arrange
        _viewModel.Text = "encrypted_text";
        _viewModel.IsLoading = true;

        // Act
        var command = _viewModel.CopyTextCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void PasteTextCommand_WhenNotLoading_CanExecute()
    {
        // Arrange
        _viewModel.IsLoading = false;

        // Act
        var command = _viewModel.PasteTextCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.True(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void PasteTextCommand_WhenLoading_CannotExecute()
    {
        // Arrange
        _viewModel.IsLoading = true;

        // Act
        var command = _viewModel.PasteTextCommand;
        var canExecute = command.CanExecute(null);

        // Assert
        Assert.False(canExecute);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task CopyTextAsync_WithValidText_SetsTextToClipboard()
    {
        // Arrange
        _viewModel.Text = "encrypted_text_to_copy";
        _clipboardServiceMock.Setup(x => x.SetTextAsync("encrypted_text_to_copy"))
            .Returns(Task.CompletedTask);

        // Act
        var command = _viewModel.CopyTextCommand;
        command.Execute(null);

        // Give it a moment to complete
        await Task.Delay(100);

        // Assert
        _clipboardServiceMock.Verify(x => x.SetTextAsync("encrypted_text_to_copy"), Times.Once);
        Assert.Equal("Text copied to clipboard!", _viewModel.StatusMessage);
        Assert.False(_viewModel.IsLoading);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task PasteTextAsync_WithClipboardText_SetsTextProperty()
    {
        // Arrange
        var clipboardText = "text_from_clipboard";
        _clipboardServiceMock.Setup(x => x.GetTextAsync())
            .ReturnsAsync(clipboardText);

        // Act
        var command = _viewModel.PasteTextCommand;
        command.Execute(null);

        // Give it a moment to complete
        await Task.Delay(100);

        // Assert
        _clipboardServiceMock.Verify(x => x.GetTextAsync(), Times.Once);
        Assert.Equal(clipboardText, _viewModel.Text);
        Assert.Equal("Text pasted from clipboard!", _viewModel.StatusMessage);
        Assert.False(_viewModel.IsLoading);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task PasteTextAsync_WithEmptyClipboard_ShowsEmptyMessage()
    {
        // Arrange
        _clipboardServiceMock.Setup(x => x.GetTextAsync())
            .ReturnsAsync(string.Empty);

        // Act
        var command = _viewModel.PasteTextCommand;
        command.Execute(null);

        // Give it a moment to complete
        await Task.Delay(100);

        // Assert
        _clipboardServiceMock.Verify(x => x.GetTextAsync(), Times.Once);
        Assert.Equal("Clipboard is empty or contains no text.", _viewModel.StatusMessage);
        Assert.False(_viewModel.IsLoading);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task CopyTextAsync_WhenClipboardFails_ShowsError()
    {
        // Arrange
        _viewModel.Text = "encrypted_text";
        _clipboardServiceMock.Setup(x => x.SetTextAsync(It.IsAny<string>()))
            .ThrowsAsync(new Exception("Clipboard access denied"));

        // Act
        var command = _viewModel.CopyTextCommand;
        command.Execute(null);

        // Give it a moment to complete
        await Task.Delay(100);

        // Assert
        Assert.StartsWith("Failed to copy to clipboard:", _viewModel.StatusMessage);
        Assert.False(_viewModel.IsLoading);
    }
}
