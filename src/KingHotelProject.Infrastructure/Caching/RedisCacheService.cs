using KingHotelProject.Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace KingHotelProject.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _cache;

        public RedisCacheService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            _cache = _redisConnection.GetDatabase();
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _cache.StringGetAsync(key);
            return value.HasValue ? JsonSerializer.Deserialize<T>(value) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _cache.StringSetAsync(key, JsonSerializer.Serialize(value), expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }
    }
}