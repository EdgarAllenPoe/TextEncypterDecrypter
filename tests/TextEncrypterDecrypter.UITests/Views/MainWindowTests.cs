using Moq;
using TextEncrypterDecrypter.Core.Services;
using TextEncrypterDecrypter.Core.ViewModels;
using TextEncrypterDecrypter.UITests.Common;
using Xunit;

namespace TextEncrypterDecrypter.UITests.Views;

/// <summary>
/// UI tests for main window - testing ViewModel integration and UI-related functionality
/// </summary>
public class MainWindowTests
{
    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_Integration_Works()
    {
        // Arrange
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var viewModel = new MainViewModel(encryptionServiceMock.Object, settingsServiceMock.Object);
        
        // Act & Assert - Test that the ViewModel can be used with UI binding
        Assert.NotNull(viewModel);
        Assert.NotNull(viewModel.Text);
        Assert.NotNull(viewModel.Password);
        Assert.NotNull(viewModel.EncryptedText);
        Assert.NotNull(viewModel.StatusMessage);
        Assert.False(viewModel.IsLoading);
        
        // Test command properties
        Assert.NotNull(viewModel.EncryptCommand);
        Assert.NotNull(viewModel.DecryptCommand);
        
        // Test initial command states
        Assert.False(viewModel.EncryptCommand.CanExecute(null)); // No text/password
        Assert.False(viewModel.DecryptCommand.CanExecute(null)); // No encrypted text/password
    }

    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_PropertyBinding_Works()
    {
        // Arrange
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var viewModel = new MainViewModel(encryptionServiceMock.Object, settingsServiceMock.Object);
        
        // Act - Simulate UI property changes
        viewModel.Text = "Test text";
        viewModel.Password = "testpassword";
        viewModel.EncryptedText = "encrypted_result";
        viewModel.StatusMessage = "Test status";
        viewModel.IsLoading = true;
        
        // Assert - Verify properties are updated
        Assert.Equal("Test text", viewModel.Text);
        Assert.Equal("testpassword", viewModel.Password);
        Assert.Equal("encrypted_result", viewModel.EncryptedText);
        Assert.Equal("Test status", viewModel.StatusMessage);
        Assert.True(viewModel.IsLoading);
        
        // Test command states after property changes
        Assert.True(viewModel.EncryptCommand.CanExecute(null)); // Has text and password
        Assert.True(viewModel.DecryptCommand.CanExecute(null)); // Has encrypted text and password
    }

    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_CommandExecution_UpdatesUI()
    {
        // Arrange
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var viewModel = new MainViewModel(encryptionServiceMock.Object, settingsServiceMock.Object);
        
        encryptionServiceMock.Setup(x => x.EncryptAsync("test", "password"))
            .ReturnsAsync("encrypted_result");
        
        // Act - Simulate UI interaction
        viewModel.Text = "test";
        viewModel.Password = "password";
        viewModel.EncryptCommand.Execute(null);
        
        // Assert - Verify UI state changes
        Assert.Equal("encrypted_result", viewModel.EncryptedText);
        Assert.Equal("Text encrypted successfully!", viewModel.StatusMessage);
        Assert.False(viewModel.IsLoading);
    }

    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_ErrorHandling_UpdatesUI()
    {
        // Arrange
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var viewModel = new MainViewModel(encryptionServiceMock.Object, settingsServiceMock.Object);
        
        encryptionServiceMock.Setup(x => x.EncryptAsync("test", "password"))
            .ThrowsAsync(new Exception("Test error"));
        
        // Act - Simulate UI interaction with error
        viewModel.Text = "test";
        viewModel.Password = "password";
        viewModel.EncryptCommand.Execute(null);
        
        // Assert - Verify error state is reflected in UI
        Assert.StartsWith("Encryption failed: Test error", viewModel.StatusMessage);
        Assert.False(viewModel.IsLoading);
    }

    [Fact]
    [Trait("Category", TestCategories.UI)]
    public async Task MainWindow_ViewModel_LoadingState_UpdatesUI()
    {
        // Arrange
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var viewModel = new MainViewModel(encryptionServiceMock.Object, settingsServiceMock.Object);
        
        // Set up a task that takes time to complete
        var tcs = new TaskCompletionSource<string>();
        encryptionServiceMock.Setup(x => x.EncryptAsync("test", "password"))
            .Returns(tcs.Task);
        
        // Act - Start encryption (async)
        viewModel.Text = "test";
        viewModel.Password = "password";
        var task = Task.Run(() => viewModel.EncryptCommand.Execute(null));
        
        // Give it a moment to start
        await Task.Delay(50);
        
        // Assert - Verify loading state is active
        Assert.True(viewModel.IsLoading);
        Assert.Equal("Encrypting...", viewModel.StatusMessage);
        
        // Complete the task
        tcs.SetResult("result");
        await task;
        
        // Assert - Verify loading state is cleared
        Assert.False(viewModel.IsLoading);
    }

    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_DataContext_CanBeSet()
    {
        // Arrange
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var viewModel = new MainViewModel(encryptionServiceMock.Object, settingsServiceMock.Object);
        
        // Act & Assert - Test that ViewModel can be used as DataContext
        Assert.NotNull(viewModel);
        
        // Simulate setting as DataContext (what the UI would do)
        var dataContext = viewModel as object;
        Assert.NotNull(dataContext);
        Assert.IsType<MainViewModel>(dataContext);
        
        // Verify all required properties are accessible for binding
        var mainViewModel = (MainViewModel)dataContext;
        Assert.NotNull(mainViewModel.Text);
        Assert.NotNull(mainViewModel.Password);
        Assert.NotNull(mainViewModel.EncryptedText);
        Assert.NotNull(mainViewModel.StatusMessage);
        Assert.NotNull(mainViewModel.EncryptCommand);
        Assert.NotNull(mainViewModel.DecryptCommand);
    }
}
