using System.Text.Json;
using TextEncrypterDecrypter.Core.Models;

namespace TextEncrypterDecrypter.Core.Services;

/// <summary>
/// JSON-based implementation of settings service
/// </summary>
public class JsonSettingsService : ISettingsService
{
    private readonly string _settingsPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public JsonSettingsService()
    {
        // Store settings in the application directory for portability
        var appDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? Environment.CurrentDirectory;
        _settingsPath = Path.Combine(appDirectory, "settings.json");
        
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    /// <inheritdoc />
    public async Task<AppSettings> LoadAsync()
    {
        try
        {
            if (!File.Exists(_settingsPath))
            {
                return new AppSettings();
            }

            var json = await File.ReadAllTextAsync(_settingsPath);
            var settings = JsonSerializer.Deserialize<AppSettings>(json, _jsonOptions);
            return settings ?? new AppSettings();
        }
        catch (Exception)
        {
            // If we can't load settings, return defaults
            return new AppSettings();
        }
    }

    /// <inheritdoc />
    public async Task SaveAsync(AppSettings settings)
    {
        try
        {
            settings.LastUsed = DateTime.Now;
            var json = JsonSerializer.Serialize(settings, _jsonOptions);
            
            // Ensure directory exists
            var directory = Path.GetDirectoryName(_settingsPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            
            await File.WriteAllTextAsync(_settingsPath, json);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to save settings to {_settingsPath}", ex);
        }
    }

    /// <inheritdoc />
    public virtual string GetSettingsPath()
    {
        return _settingsPath;
    }
}
