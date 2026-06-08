namespace ToMainApi.Interfaces
{
    public interface IEncryptService
    {
        public string Encrypt(string text);
        public string Decrypt(string text);
    }
}
