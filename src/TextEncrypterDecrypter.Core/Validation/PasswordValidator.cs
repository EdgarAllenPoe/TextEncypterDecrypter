using System.Text.RegularExpressions;

namespace TextEncrypterDecrypter.Core.Validation;

/// <summary>
/// Password validator implementation
/// </summary>
public class PasswordValidator : IPasswordValidator
{
    private const int MinimumLength = 8;

    /// <inheritdoc />
    public PasswordValidationResult ValidatePassword(string password)
    {
        if (password == null)
            throw new ArgumentException("Password cannot be null", nameof(password));

        var result = new PasswordValidationResult();

        // Check minimum length
        if (string.IsNullOrEmpty(password))
        {
            result.Messages.Add("Password cannot be empty");
            return result;
        }

        if (password.Length < MinimumLength)
        {
            result.Messages.Add("Password must be at least 8 characters long");
            return result;
        }

        // Calculate strength score
        result.StrengthScore = CalculateStrengthScore(password);
        result.Strength = GetStrengthFromScore(result.StrengthScore);
        result.IsValid = true;

        // Add feedback messages
        AddFeedbackMessages(password, result);

        return result;
    }

    private int CalculateStrengthScore(string password)
    {
        int score = 0;

        // Length bonus (up to 30 points)
        score += Math.Min(password.Length * 3, 30);

        // Character variety bonuses
        if (HasLowercase(password)) score += 15;
        if (HasUppercase(password)) score += 15;
        if (HasNumbers(password)) score += 15;
        if (HasSpecialCharacters(password)) score += 20;

        // Complexity bonus
        if (password.Length >= 12) score += 15;
        if (password.Length >= 16) score += 15;

        // Penalties (reduced)
        if (HasRepeatingCharacters(password)) score -= 5;
        if (HasCommonPatterns(password)) score -= 10;

        return Math.Max(0, Math.Min(100, score));
    }

    private PasswordStrength GetStrengthFromScore(int score)
    {
        return score switch
        {
            < 20 => PasswordStrength.VeryWeak,
            < 40 => PasswordStrength.Weak,
            < 70 => PasswordStrength.Medium,
            < 90 => PasswordStrength.Strong,
            _ => PasswordStrength.VeryStrong
        };
    }

    private void AddFeedbackMessages(string password, PasswordValidationResult result)
    {
        if (password.Length < 8)
        {
            result.Messages.Add("Password must be at least 8 characters long");
        }

        if (!HasLowercase(password) && !HasUppercase(password))
        {
            result.Messages.Add("Password should contain letters");
        }

        if (!HasNumbers(password))
        {
            result.Messages.Add("Password should contain numbers");
        }

        if (!HasSpecialCharacters(password))
        {
            result.Messages.Add("Password should contain special characters");
        }

        if (HasLowercase(password) && !HasUppercase(password))
        {
            result.Messages.Add("Password contains only lowercase letters");
        }

        if (!HasLowercase(password) && HasUppercase(password))
        {
            result.Messages.Add("Password contains only uppercase letters");
        }

        if (HasNumbers(password) && !HasLowercase(password) && !HasUppercase(password))
        {
            result.Messages.Add("Password contains only numbers");
        }

        if (HasRepeatingCharacters(password))
        {
            result.Messages.Add("Password contains repeating characters");
        }

        if (HasCommonPatterns(password))
        {
            result.Messages.Add("Password contains common patterns");
        }
    }

    private bool HasLowercase(string password) => password.Any(char.IsLower);
    private bool HasUppercase(string password) => password.Any(char.IsUpper);
    private bool HasNumbers(string password) => password.Any(char.IsDigit);
    private bool HasSpecialCharacters(string password) => password.Any(c => !char.IsLetterOrDigit(c));

    private bool HasRepeatingCharacters(string password)
    {
        for (int i = 0; i < password.Length - 2; i++)
        {
            if (password[i] == password[i + 1] && password[i] == password[i + 2])
                return true;
        }
        return false;
    }

    private bool HasCommonPatterns(string password)
    {
        var commonPatterns = new[]
        {
            "123", "abc", "qwe", "asd", "password", "admin", "user", "test"
        };

        return commonPatterns.Any(pattern => 
            password.ToLower().Contains(pattern));
    }
}
