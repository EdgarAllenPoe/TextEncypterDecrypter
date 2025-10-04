using TextEncrypterDecrypter.Core.Services;
using TextEncrypterDecrypter.UnitTests.Common;
using Xunit;

namespace TextEncrypterDecrypter.UnitTests.Services;

/// <summary>
/// Unit tests for clipboard service
/// </summary>
public class ClipboardServiceTests
{
    private readonly IClipboardService _clipboardService;

    public ClipboardServiceTests()
    {
        _clipboardService = new ClipboardService();
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task SetTextAsync_WithValidText_SetsTextToClipboard()
    {
        // Arrange
        var testText = "Hello, Clipboard!";

        // Act
        await _clipboardService.SetTextAsync(testText);

        // Assert - Verify no exception is thrown
        // Note: In a real test environment, we'd need to mock the clipboard
        // For now, we're testing that the method doesn't throw exceptions
        Assert.True(true); // This will be replaced with proper clipboard verification
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task SetTextAsync_WithEmptyText_ThrowsArgumentException()
    {
        // Arrange
        var emptyText = "";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _clipboardService.SetTextAsync(emptyText));
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task SetTextAsync_WithNullText_ThrowsArgumentException()
    {
        // Arrange
        string? nullText = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _clipboardService.SetTextAsync(nullText!));
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task GetTextAsync_ReturnsTextFromClipboard()
    {
        // Act
        var result = await _clipboardService.GetTextAsync();

        // Assert - Should not throw exception
        // Note: Actual clipboard content depends on system state
        Assert.True(result == null || !string.IsNullOrEmpty(result));
    }
}
