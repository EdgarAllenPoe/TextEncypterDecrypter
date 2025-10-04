namespace TextEncrypterDecrypter.Core.Services;

/// <summary>
/// Service interface for clipboard operations
/// </summary>
public interface IClipboardService
{
    /// <summary>
    /// Sets text to the clipboard
    /// </summary>
    /// <param name="text">The text to copy to clipboard</param>
    /// <returns>A task representing the async operation</returns>
    Task SetTextAsync(string text);

    /// <summary>
    /// Gets text from the clipboard
    /// </summary>
    /// <returns>The text from clipboard, or null if clipboard is empty or doesn't contain text</returns>
    Task<string?> GetTextAsync();
}
