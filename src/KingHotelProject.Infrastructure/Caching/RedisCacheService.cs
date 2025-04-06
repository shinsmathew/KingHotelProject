using KingHotelProject.Core.Interfaces;
using StackExchange.Redis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KingHotelProject.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _cache;
        private readonly JsonSerializerOptions _jsonOptions;

        public RedisCacheService(IConnectionMultiplexer redisConnection)
        {
            _redisConnection = redisConnection;
            _cache = _redisConnection.GetDatabase();

            _jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var value = await _cache.StringGetAsync(key);
            return value.HasValue ?
                JsonSerializer.Deserialize<T>(value, _jsonOptions) : default;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _cache.StringSetAsync(
                           key,
                           JsonSerializer.Serialize(value, _jsonOptions),
                           expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _cache.KeyDeleteAsync(key);
        }
    }
}