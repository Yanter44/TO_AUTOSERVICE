namespace ToMainApi.Interfaces
{
    public interface IArchiveCryptoService
    {
        Task<byte[]> EncryptAsync(IFormFile file, byte[] masterKey, CancellationToken ct = default);
        byte[] Decrypt(byte[] encrypted, byte[] masterKey);
    }
}
