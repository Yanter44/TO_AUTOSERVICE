namespace ToMainApi.Interfaces
{
    public interface IInfisicalService
    {
        Task<string> GetSecretAsync(string secretName);
    }
}
