using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace TextEncrypterDecrypter.Core.Validation;

/// <summary>
/// Service for validating password strength and providing feedback.
/// </summary>
public class PasswordValidator : IPasswordValidator
{
    /// <inheritdoc />
    public PasswordValidationResult ValidatePassword(string password)
    {
        var result = new PasswordValidationResult
        {
            IsValid = true,
            Strength = PasswordStrength.VeryWeak
        };

        if (password == null)
        {
            throw new ArgumentException("Password cannot be null", nameof(password));
        }

        if (string.IsNullOrEmpty(password))
        {
            result.IsValid = false;
            result.Messages.Add("Password cannot be empty");
            return result;
        }

        if (password.Length < 8)
        {
            result.IsValid = false;
        }

        int score = CalculateStrengthScore(password);
        result.StrengthScore = score;
        result.Strength = GetStrengthFromScore(score);

        AddFeedbackMessages(password, result);

        return result;
    }

    private int CalculateStrengthScore(string password)
    {
        int score = 0;

        // Length bonus (up to 25 points)
        score += Math.Min((int)(password.Length * 2.5), 25);

        // Character variety bonuses (balanced)
        if (HasLowercase(password)) score += 10;
        if (HasUppercase(password)) score += 10;
        if (HasNumbers(password)) score += 10;
        if (HasSpecialCharacters(password)) score += 15;

        // Complexity bonus (balanced)
        if (password.Length >= 12) score += 10;
        if (password.Length >= 16) score += 15;

        // Penalties
        if (HasRepeatingCharacters(password)) score -= 5;
        if (HasCommonPatterns(password)) score -= 5;

        return Math.Max(0, Math.Min(100, score));
    }

    private PasswordStrength GetStrengthFromScore(int score)
    {
        return score switch
        {
            < 20 => PasswordStrength.VeryWeak,
            < 40 => PasswordStrength.Weak,
            < 65 => PasswordStrength.Medium,
            < 85 => PasswordStrength.Strong,
            _ => PasswordStrength.VeryStrong
        };
    }

    private void AddFeedbackMessages(string password, PasswordValidationResult result)
    {
        if (password.Length < 8)
        {
            result.Messages.Add("Password must be at least 8 characters long");
        }

        // Check for "only numbers" case first
        if (HasNumbers(password) && !HasLowercase(password) && !HasUppercase(password) && !HasSpecialCharacters(password))
        {
            result.Messages.Add("Password contains only numbers");
            return; // Don't add other messages for this case
        }

        if (!HasLowercase(password))
        {
            result.Messages.Add("Include lowercase letters.");
        }
        if (!HasUppercase(password))
        {
            result.Messages.Add("Include uppercase letters.");
        }
        if (!HasNumbers(password))
        {
            result.Messages.Add("Include numbers.");
        }
        if (!HasSpecialCharacters(password))
        {
            result.Messages.Add("Include special characters.");
        }
        if (HasCommonPatterns(password))
        {
            result.Messages.Add("Avoid common patterns (e.g., 'password', '123').");
        }
        if (HasRepeatingCharacters(password))
        {
            result.Messages.Add("Avoid repeating characters (e.g., 'aaa').");
        }
    }

    private bool HasLowercase(string password) => password.Any(char.IsLower);
    private bool HasUppercase(string password) => password.Any(char.IsUpper);
    private bool HasNumbers(string password) => password.Any(char.IsDigit);
    private bool HasSpecialCharacters(string password) => password.Any(ch => !char.IsLetterOrDigit(ch));
    private bool HasRepeatingCharacters(string password) => Regex.IsMatch(password, @"(.)\1{2,}"); // Three or more repeating characters
    private bool HasCommonPatterns(string password)
    {
        var commonPatterns = new[] { "password", "123", "abc", "qwerty", "admin" };
        return commonPatterns.Any(p => password.Contains(p, StringComparison.OrdinalIgnoreCase));
    }
}
