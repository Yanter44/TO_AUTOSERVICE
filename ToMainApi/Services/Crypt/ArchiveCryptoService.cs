using System.Security.Cryptography;
using ToMainApi.Interfaces;

namespace ToMainApi.Services.Crypt
{
    public sealed class ArchiveCryptoService : IArchiveCryptoService
    {
        public async Task<byte[]> EncryptAsync(IFormFile file, byte[] masterKey, CancellationToken ct = default)
        {
            if (file is null || file.Length == 0)
                throw new ArgumentException("Файл не приложен или пуст", nameof(file));

            byte[] plaintext;
            await using (var stream = file.OpenReadStream())
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms, ct);
                plaintext = ms.ToArray();
            }

            try
            {
                var nonce = RandomNumberGenerator.GetBytes(12);
                var tag = new byte[16];
                var ciphertext = new byte[plaintext.Length];

                using var aes = new AesGcm(masterKey, 16);
                aes.Encrypt(nonce, plaintext, ciphertext, tag);

                var result = new byte[12 + 16 + ciphertext.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, 12);
                Buffer.BlockCopy(tag, 0, result, 12, 16);
                Buffer.BlockCopy(ciphertext, 0, result, 28, ciphertext.Length);
                return result;
            }
            finally
            {
                CryptographicOperations.ZeroMemory(plaintext);
            }
        }

        public byte[] Decrypt(byte[] encrypted, byte[] masterKey)
        {
            if (encrypted is null || encrypted.Length < 28)
                throw new ArgumentException("Некорректные зашифрованные данные", nameof(encrypted));

            var nonce = encrypted.AsSpan(0, 12);
            var tag = encrypted.AsSpan(12, 16);
            var ciphertext = encrypted.AsSpan(28);

            var plaintext = new byte[ciphertext.Length];
            using var aes = new AesGcm(masterKey, 16);
            aes.Decrypt(nonce, ciphertext, tag, plaintext);
            return plaintext;
        }
    }
}
