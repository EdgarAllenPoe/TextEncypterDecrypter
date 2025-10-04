using System.Security.Cryptography;
using System.Text;

namespace TextEncrypterDecrypter.Core.Services;

/// <summary>
/// AES-based encryption service implementation
/// </summary>
public class EncryptionService : IEncryptionService
{
    private const int KeySize = 256;
    private const int IvSize = 128;
    private const int SaltSize = 256;
    private const int Iterations = 100000;

    /// <inheritdoc />
    public async Task<string> EncryptAsync(string text, string password)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentException("Text cannot be null or empty", nameof(text));
        
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        return await Task.Run(() =>
        {
            // Generate random salt and IV
            var salt = new byte[SaltSize / 8];
            var iv = new byte[IvSize / 8];
            
            RandomNumberGenerator.Fill(salt);
            RandomNumberGenerator.Fill(iv);

            // Derive key from password using PBKDF2
            using var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.BlockSize = IvSize;
            aes.Key = key.GetBytes(KeySize / 8);
            aes.IV = iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            // Encrypt the text
            using var encryptor = aes.CreateEncryptor();
            using var msEncrypt = new MemoryStream();
            using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
            using var swEncrypt = new StreamWriter(csEncrypt, Encoding.UTF8);
            
            swEncrypt.Write(text);
            swEncrypt.Close();
            
            var encryptedBytes = msEncrypt.ToArray();

            // Combine salt + IV + encrypted data
            var result = new byte[salt.Length + iv.Length + encryptedBytes.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
            Buffer.BlockCopy(encryptedBytes, 0, result, salt.Length + iv.Length, encryptedBytes.Length);

            return Convert.ToBase64String(result);
        });
    }

    /// <inheritdoc />
    public async Task<string> DecryptAsync(string encryptedText, string password)
    {
        if (string.IsNullOrEmpty(encryptedText))
            throw new ArgumentException("Encrypted text cannot be null or empty", nameof(encryptedText));
        
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password cannot be null or empty", nameof(password));

        return await Task.Run(() =>
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                
                if (encryptedBytes.Length < SaltSize / 8 + IvSize / 8)
                    throw new CryptographicException("Invalid encrypted data format");

                // Extract salt, IV, and encrypted data
                var salt = new byte[SaltSize / 8];
                var iv = new byte[IvSize / 8];
                var cipherText = new byte[encryptedBytes.Length - salt.Length - iv.Length];

                Buffer.BlockCopy(encryptedBytes, 0, salt, 0, salt.Length);
                Buffer.BlockCopy(encryptedBytes, salt.Length, iv, 0, iv.Length);
                Buffer.BlockCopy(encryptedBytes, salt.Length + iv.Length, cipherText, 0, cipherText.Length);

                // Derive key from password using PBKDF2
                using var key = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
                using var aes = Aes.Create();
                aes.KeySize = KeySize;
                aes.BlockSize = IvSize;
                aes.Key = key.GetBytes(KeySize / 8);
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Decrypt the text
                using var decryptor = aes.CreateDecryptor();
                using var msDecrypt = new MemoryStream(cipherText);
                using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                using var srDecrypt = new StreamReader(csDecrypt, Encoding.UTF8);
                
                return srDecrypt.ReadToEnd();
            }
            catch (FormatException)
            {
                throw new CryptographicException("Invalid base64 format");
            }
            catch (Exception ex) when (!(ex is CryptographicException))
            {
                throw new CryptographicException("Decryption failed", ex);
            }
        });
    }
}
