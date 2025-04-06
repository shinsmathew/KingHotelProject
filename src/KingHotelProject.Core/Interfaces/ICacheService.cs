namespace KingHotelProject.Core.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetRedisCacheAsync<T>(string key);
        Task SetRedisCacheAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task RemoveRedisCacheAsync(string key);
    }
}