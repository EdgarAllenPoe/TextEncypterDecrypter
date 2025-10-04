using System.IO;
using System.Text.Json;
using TextEncrypterDecrypter.Core.Models;
using TextEncrypterDecrypter.Core.Services;
using TextEncrypterDecrypter.IntegrationTests.Common;
using Xunit;

namespace TextEncrypterDecrypter.IntegrationTests.Services;

/// <summary>
/// Integration tests for settings service
/// </summary>
public class SettingsServiceIntegrationTests
{
    [Fact]
    [Trait("Category", TestCategories.Integration)]
    public async Task SettingsService_CanSaveAndLoadSettings_RoundTripSuccess()
    {
        // Arrange
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);
        
        try
        {
            var originalSettings = new AppSettings
            {
                LastUsedPassword = "testpassword123",
                LastEncryptedText = "encrypted_data_here",
                LastUsed = DateTime.Now,
                RememberPassword = true,
                Version = "1.0.0"
            };

            // Create a settings service that uses the temp directory
            var settingsService = new TestJsonSettingsService(tempDirectory);

            // Act - Save and then load settings
            await settingsService.SaveAsync(originalSettings);
            var loadedSettings = await settingsService.LoadAsync();

            // Assert
            Assert.NotNull(loadedSettings);
            Assert.Equal(originalSettings.LastUsedPassword, loadedSettings.LastUsedPassword);
            Assert.Equal(originalSettings.LastEncryptedText, loadedSettings.LastEncryptedText);
            Assert.Equal(originalSettings.RememberPassword, loadedSettings.RememberPassword);
            Assert.Equal(originalSettings.Version, loadedSettings.Version);
            
            // LastUsed should be updated during save
            Assert.True(loadedSettings.LastUsed >= originalSettings.LastUsed);
        }
        finally
        {
            // Cleanup
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }

    [Fact]
    [Trait("Category", TestCategories.Integration)]
    public async Task SettingsService_WithMissingFile_ReturnsDefaultSettings()
    {
        // Arrange
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);
        
        try
        {
            var settingsService = new TestJsonSettingsService(tempDirectory);

            // Act
            var settings = await settingsService.LoadAsync();

            // Assert
            Assert.NotNull(settings);
            Assert.Null(settings.LastUsedPassword);
            Assert.Null(settings.LastEncryptedText);
            Assert.False(settings.RememberPassword);
            Assert.Equal("1.0.0", settings.Version);
            Assert.True(settings.LastUsed <= DateTime.Now);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }

    [Fact]
    [Trait("Category", TestCategories.Integration)]
    public async Task SettingsService_WithCorruptedFile_ReturnsDefaultSettings()
    {
        // Arrange
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(tempDirectory);
        var settingsPath = Path.Combine(tempDirectory, "settings.json");
        
        try
        {
            // Create a corrupted JSON file
            await File.WriteAllTextAsync(settingsPath, "invalid json content {");

            var settingsService = new TestJsonSettingsService(tempDirectory);

            // Act
            var settings = await settingsService.LoadAsync();

            // Assert
            Assert.NotNull(settings);
            Assert.Null(settings.LastUsedPassword);
            Assert.Null(settings.LastEncryptedText);
            Assert.False(settings.RememberPassword);
            Assert.Equal("1.0.0", settings.Version);
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }

    [Fact]
    [Trait("Category", TestCategories.Integration)]
    public async Task SettingsService_CanCreateDirectory_WhenDirectoryDoesNotExist()
    {
        // Arrange
        var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var subDirectory = Path.Combine(tempDirectory, "subdir");
        var settingsPath = Path.Combine(subDirectory, "settings.json");
        
        try
        {
            var settingsService = new TestJsonSettingsService(subDirectory);
            var settings = new AppSettings { LastUsedPassword = "test" };

            // Act
            await settingsService.SaveAsync(settings);

            // Assert
            Assert.True(Directory.Exists(subDirectory));
            Assert.True(File.Exists(settingsPath));
        }
        finally
        {
            if (Directory.Exists(tempDirectory))
            {
                Directory.Delete(tempDirectory, true);
            }
        }
    }
}

/// <summary>
/// Test implementation of JsonSettingsService that allows custom directory
/// </summary>
internal class TestJsonSettingsService : ISettingsService
{
    private readonly string _settingsPath;
    private readonly JsonSerializerOptions _jsonOptions;

    public TestJsonSettingsService(string directory)
    {
        _settingsPath = Path.Combine(directory, "settings.json");
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

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

    public string GetSettingsPath()
    {
        return _settingsPath;
    }
}
