namespace TextEncrypterDecrypter.Core.Services;

/// <summary>
/// Service interface for encrypting and decrypting text
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the provided text using the given password
    /// </summary>
    /// <param name="text">The text to encrypt</param>
    /// <param name="password">The password to use for encryption</param>
    /// <returns>A base64-encoded encrypted string</returns>
    Task<string> EncryptAsync(string text, string password);

    /// <summary>
    /// Decrypts the provided encrypted text using the given password
    /// </summary>
    /// <param name="encryptedText">The base64-encoded encrypted text</param>
    /// <param name="password">The password used for encryption</param>
    /// <returns>The decrypted original text</returns>
    Task<string> DecryptAsync(string encryptedText, string password);
}
