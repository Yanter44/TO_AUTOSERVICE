namespace ToMainApi.Interfaces
{
    public interface IRedisService
    {
        Task SetStringAsync(string key, string value, TimeSpan? ttl = null);
        Task<string?> GetStringAsync(string key);
        Task DeleteAsync(string key);

        Task SetObjectAsync<T>(string key, T value, TimeSpan? ttl = null);
        Task<T?> GetObjectAsync<T>(string key);
    }
}
