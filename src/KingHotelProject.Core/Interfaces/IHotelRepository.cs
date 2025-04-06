using KingHotelProject.Core.Entities;

namespace KingHotelProject.Core.Interfaces
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Hotel>> GetAllAsync();
        Task<Hotel> GetByIdAsync(Guid id);
        Task<Hotel> AddAsync(Hotel hotel);
        Task UpdateAsync(Hotel hotel);
        Task DeleteAsync(Hotel hotel);
    }
}