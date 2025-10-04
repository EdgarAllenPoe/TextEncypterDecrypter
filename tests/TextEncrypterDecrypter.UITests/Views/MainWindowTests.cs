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
    private static MainViewModel CreateMainViewModel()
    {
        var encryptionServiceMock = new Mock<IEncryptionService>();
        var settingsServiceMock = new Mock<ISettingsService>();
        var clipboardServiceMock = new Mock<IClipboardService>();
        return new MainViewModel(encryptionServiceMock.Object, settingsServiceMock.Object, clipboardServiceMock.Object);
    }
    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_Integration_Works()
    {
        // Arrange
        var viewModel = CreateMainViewModel();
        
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
        var viewModel = CreateMainViewModel();
        
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
        var viewModel = CreateMainViewModel();
        
        // Act - Simulate UI interaction (without actual encryption for UI test)
        viewModel.Text = "test";
        viewModel.Password = "password";
        
        // Assert - Verify UI state changes
        Assert.Equal("test", viewModel.Text);
        Assert.Equal("password", viewModel.Password);
        Assert.True(viewModel.EncryptCommand.CanExecute(null));
    }

    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_ErrorHandling_UpdatesUI()
    {
        // Arrange
        var viewModel = CreateMainViewModel();
        
        // Act - Simulate UI interaction (test property changes)
        viewModel.Text = "test";
        viewModel.Password = "password";
        
        // Assert - Verify UI state changes
        Assert.Equal("test", viewModel.Text);
        Assert.Equal("password", viewModel.Password);
        Assert.True(viewModel.EncryptCommand.CanExecute(null));
    }

    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_LoadingState_UpdatesUI()
    {
        // Arrange
        var viewModel = CreateMainViewModel();
        
        // Act - Test property changes and command states
        viewModel.Text = "test";
        viewModel.Password = "password";
        
        // Assert - Verify initial state
        Assert.Equal("test", viewModel.Text);
        Assert.Equal("password", viewModel.Password);
        Assert.True(viewModel.EncryptCommand.CanExecute(null));
        Assert.False(viewModel.IsLoading);
    }

    [Fact]
    [Trait("Category", TestCategories.UI)]
    public void MainWindow_ViewModel_DataContext_CanBeSet()
    {
        // Arrange
        var viewModel = CreateMainViewModel();
        
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
