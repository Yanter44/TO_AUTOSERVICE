namespace ToMainApi.Services
{
    using Microsoft.EntityFrameworkCore.Storage;
    using StackExchange.Redis;
    using System.Text.Json;
    using ToMainApi.Interfaces;

    public class RedisService : IRedisService
    {
        private readonly StackExchange.Redis.IDatabase _db;

        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SetStringAsync(string key, string value, TimeSpan? ttl = null)
        {
            await _db.StringSetAsync(key, value, (Expiration)ttl);
        }

        public async Task<string?> GetStringAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task DeleteAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task SetObjectAsync<T>(string key, T value, TimeSpan? ttl = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, (Expiration)ttl);
        }

        public async Task<T?> GetObjectAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>((string)value!);
        }
    }
}
