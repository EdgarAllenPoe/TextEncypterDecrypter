using TextEncrypterDecrypter.Core.Validation;
using TextEncrypterDecrypter.UnitTests.Common;
using Xunit;

namespace TextEncrypterDecrypter.UnitTests.Validation;

/// <summary>
/// Unit tests for password validator
/// </summary>
public class PasswordValidatorTests
{
    private readonly IPasswordValidator _passwordValidator;

    public PasswordValidatorTests()
    {
        _passwordValidator = new PasswordValidator();
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void ValidatePassword_WithEmptyPassword_ReturnsInvalid()
    {
        // Arrange
        var password = "";

        // Act
        var result = _passwordValidator.ValidatePassword(password);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(0, result.StrengthScore);
        Assert.Equal(PasswordStrength.VeryWeak, result.Strength);
        Assert.Contains("Password cannot be empty", result.Messages);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void ValidatePassword_WithShortPassword_ReturnsInvalid()
    {
        // Arrange
        var password = "123";

        // Act
        var result = _passwordValidator.ValidatePassword(password);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.StrengthScore < 20);
        Assert.Equal(PasswordStrength.VeryWeak, result.Strength);
        Assert.Contains("Password must be at least 8 characters long", result.Messages);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void ValidatePassword_WithWeakPassword_ReturnsValidButWeak()
    {
        // Arrange
        var password = "12345678";

        // Act
        var result = _passwordValidator.ValidatePassword(password);

        // Assert
        Assert.True(result.IsValid);
        Assert.True(result.StrengthScore < 40);
        Assert.Equal(PasswordStrength.Weak, result.Strength);
        Assert.Contains("Password contains only numbers", result.Messages);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void ValidatePassword_WithMediumPassword_ReturnsValidAndMedium()
    {
        // Arrange
        var password = "password123";

        // Act
        var result = _passwordValidator.ValidatePassword(password);

        // Assert
        Assert.True(result.IsValid);
        Assert.True(result.StrengthScore >= 40 && result.StrengthScore < 70);
        Assert.Equal(PasswordStrength.Medium, result.Strength);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void ValidatePassword_WithStrongPassword_ReturnsValidAndStrong()
    {
        // Arrange
        var password = "MyPassword123!";

        // Act
        var result = _passwordValidator.ValidatePassword(password);

        // Assert
        Assert.True(result.IsValid);
        Assert.True(result.StrengthScore >= 70 && result.StrengthScore < 90);
        Assert.Equal(PasswordStrength.Strong, result.Strength);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void ValidatePassword_WithVeryStrongPassword_ReturnsValidAndVeryStrong()
    {
        // Arrange
        var password = "MyVeryStrongPassword123!@#";

        // Act
        var result = _passwordValidator.ValidatePassword(password);

        // Assert
        Assert.True(result.IsValid);
        Assert.True(result.StrengthScore >= 90);
        Assert.Equal(PasswordStrength.VeryStrong, result.Strength);
    }

    [Theory]
    [Trait("Category", TestCategories.Unit)]
    [InlineData("password", PasswordStrength.Weak)]
    [InlineData("Password", PasswordStrength.Weak)]
    [InlineData("PASSWORD", PasswordStrength.Weak)]
    [InlineData("password123", PasswordStrength.Medium)]
    [InlineData("Password123", PasswordStrength.Medium)]
    [InlineData("Password123!", PasswordStrength.Strong)]
    [InlineData("MyPassword123!", PasswordStrength.Strong)]
    [InlineData("MyVeryStrongPassword123!@#", PasswordStrength.VeryStrong)]
    public void ValidatePassword_WithVariousPasswords_ReturnsCorrectStrength(string password, PasswordStrength expectedStrength)
    {
        // Act
        var result = _passwordValidator.ValidatePassword(password);

        // Assert
        Assert.Equal(expectedStrength, result.Strength);
        Assert.True(result.IsValid);
    }

    [Fact]
    [Trait("Category", TestCategories.Unit)]
    public void ValidatePassword_WithNullPassword_ThrowsArgumentException()
    {
        // Arrange
        string? password = null;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _passwordValidator.ValidatePassword(password!));
    }
}
