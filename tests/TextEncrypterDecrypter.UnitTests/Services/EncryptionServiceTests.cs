using TextEncrypterDecrypter.Core.Services;
using TextEncrypterDecrypter.UnitTests.Common;
using Xunit;

namespace TextEncrypterDecrypter.UnitTests.Services;

/// <summary>
/// Unit tests for encryption service
/// </summary>
public class EncryptionServiceTests
{
    private readonly IEncryptionService _encryptionService;

    public EncryptionServiceTests()
    {
        _encryptionService = new EncryptionService();
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task EncryptText_WithValidInput_ReturnsEncryptedText()
    {
        // Arrange
        var text = "Hello, World!";
        var password = "testpassword123";

        // Act
        var encryptedText = await _encryptionService.EncryptAsync(text, password);

        // Assert
        Assert.NotNull(encryptedText);
        Assert.NotEmpty(encryptedText);
        Assert.NotEqual(text, encryptedText);
        Assert.True(encryptedText.Length > text.Length);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task DecryptText_WithValidInput_ReturnsOriginalText()
    {
        // Arrange
        var originalText = "Hello, World!";
        var password = "testpassword123";
        var encryptedText = await _encryptionService.EncryptAsync(originalText, password);

        // Act
        var decryptedText = await _encryptionService.DecryptAsync(encryptedText, password);

        // Assert
        Assert.Equal(originalText, decryptedText);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public async Task DecryptText_WithInvalidPassword_ThrowsSecurityException()
    {
        // Arrange
        var originalText = "Hello, World!";
        var correctPassword = "correctpassword";
        var wrongPassword = "wrongpassword";
        var encryptedText = await _encryptionService.EncryptAsync(originalText, correctPassword);

        // Act & Assert
        await Assert.ThrowsAsync<System.Security.Cryptography.CryptographicException>(
            () => _encryptionService.DecryptAsync(encryptedText, wrongPassword));
    }

    [Theory]
    [Trait("Category", TestCategories.Unit)]
    [InlineData("")]
    [InlineData(null)]
    public async Task EncryptText_WithEmptyOrNullText_ThrowsArgumentException(string? text)
    {
        // Arrange
        var password = "testpassword123";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _encryptionService.EncryptAsync(text!, password));
    }

    [Theory]
    [Trait("Category", TestCategories.Unit)]
    [InlineData("")]
    [InlineData(null)]
    public async Task EncryptText_WithEmptyOrNullPassword_ThrowsArgumentException(string? password)
    {
        // Arrange
        var text = "Hello, World!";

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _encryptionService.EncryptAsync(text, password!));
    }
}
