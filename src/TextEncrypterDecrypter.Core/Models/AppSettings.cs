namespace TextEncrypterDecrypter.Core.Models;

/// <summary>
/// Application settings model
/// </summary>
public class AppSettings
{
    /// <summary>
    /// The last password used for encryption/decryption
    /// </summary>
    public string? LastUsedPassword { get; set; }

    /// <summary>
    /// The last encrypted text processed
    /// </summary>
    public string? LastEncryptedText { get; set; }

    /// <summary>
    /// The last time the application was used
    /// </summary>
    public DateTime LastUsed { get; set; } = DateTime.Now;

    /// <summary>
    /// Whether to remember the password in the settings
    /// </summary>
    public bool RememberPassword { get; set; } = false;

    /// <summary>
    /// The application version when settings were last saved
    /// </summary>
    public string Version { get; set; } = "1.0.0";
}
