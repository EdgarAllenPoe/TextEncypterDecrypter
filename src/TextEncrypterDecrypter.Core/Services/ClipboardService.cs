namespace TextEncrypterDecrypter.Core.Services;

/// <summary>
/// Mock clipboard service implementation for Core layer
/// Real implementation will be in the App layer
/// </summary>
public class ClipboardService : IClipboardService
{
    private string? _storedText;

    /// <inheritdoc />
    public async Task SetTextAsync(string text)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Text cannot be null or empty", nameof(text));

        await Task.Run(() =>
        {
            try
            {
                // Store text in memory for testing
                _storedText = text;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to set clipboard text", ex);
            }
        });
    }

    /// <inheritdoc />
    public async Task<string?> GetTextAsync()
    {
        return await Task.Run(() =>
        {
            try
            {
                return _storedText;
            }
            catch (Exception)
            {
                return null; // Return null if clipboard access fails
            }
        });
    }
}
