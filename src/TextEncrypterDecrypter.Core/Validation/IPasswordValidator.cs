namespace TextEncrypterDecrypter.Core.Validation;

/// <summary>
/// Interface for password validation
/// </summary>
public interface IPasswordValidator
{
    /// <summary>
    /// Validates a password and returns validation result
    /// </summary>
    /// <param name="password">The password to validate</param>
    /// <returns>Password validation result</returns>
    PasswordValidationResult ValidatePassword(string password);
}

/// <summary>
/// Result of password validation
/// </summary>
public class PasswordValidationResult
{
    /// <summary>
    /// Whether the password is valid
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Password strength score (0-100)
    /// </summary>
    public int StrengthScore { get; set; }

    /// <summary>
    /// Password strength level
    /// </summary>
    public PasswordStrength Strength { get; set; }

    /// <summary>
    /// List of validation messages
    /// </summary>
    public List<string> Messages { get; set; } = new List<string>();
}

/// <summary>
/// Password strength levels
/// </summary>
public enum PasswordStrength
{
    VeryWeak = 0,
    Weak = 1,
    Medium = 2,
    Strong = 3,
    VeryStrong = 4
}
