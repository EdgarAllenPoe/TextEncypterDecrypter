using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using TextEncrypterDecrypter.Core.Services;

namespace TextEncrypterDecrypter.App.Services;

/// <summary>
/// Real clipboard service implementation for the App layer
/// Uses Avalonia's clipboard functionality
/// </summary>
public class ClipboardService : IClipboardService
{
    /// <inheritdoc />
    public async Task SetTextAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Text cannot be null or empty", nameof(text));

        try
        {
            var appLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var topLevel = TopLevel.GetTopLevel(appLifetime?.MainWindow);
            if (topLevel?.Clipboard != null)
            {
                await topLevel.Clipboard.SetTextAsync(text);
            }
            else
            {
                throw new InvalidOperationException("Clipboard is not available");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to set clipboard text", ex);
        }
    }

    /// <inheritdoc />
    public async Task<string?> GetTextAsync()
    {
        try
        {
            var appLifetime = Application.Current?.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime;
            var topLevel = TopLevel.GetTopLevel(appLifetime?.MainWindow);
            if (topLevel?.Clipboard != null)
            {
                return await topLevel.Clipboard.GetTextAsync();
            }
            else
            {
                return null; // Return null if clipboard access fails
            }
        }
        catch (Exception)
        {
            return null; // Return null if clipboard access fails
        }
    }
}