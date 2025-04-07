using KingHotelProject.Core.Entities;

namespace KingHotelProject.Core.Interfaces
{
    public interface IHotelRepository
    {
        Task<IEnumerable<Hotel>> GetAllHotelAsync();
        Task<Hotel> GetHotelByIdAsync(Guid id);
        Task<IEnumerable<Hotel>> GetHotelByIdsAsync(IEnumerable<Guid> ids);
        Task<Hotel> AddHotelAsync(Hotel hotel);
        Task UpdateHotelAsync(Hotel hotel);
        Task DeleteHotelAsync(Hotel hotel);
    }
}