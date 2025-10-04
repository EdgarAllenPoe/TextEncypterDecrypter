using TextEncrypterDecrypter.Core.Models;

namespace TextEncrypterDecrypter.Core.Services;

/// <summary>
/// Service interface for managing application settings
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Loads application settings from storage
    /// </summary>
    /// <returns>Application settings or default settings if none exist</returns>
    Task<AppSettings> LoadAsync();

    /// <summary>
    /// Saves application settings to storage
    /// </summary>
    /// <param name="settings">The settings to save</param>
    Task SaveAsync(AppSettings settings);

    /// <summary>
    /// Gets the path where settings are stored
    /// </summary>
    /// <returns>The settings file path</returns>
    string GetSettingsPath();
}
