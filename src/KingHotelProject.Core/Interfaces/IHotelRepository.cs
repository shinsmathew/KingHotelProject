
using KingHotelProject.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KingHotelProject.Core.Interfaces
{
    public interface IHotelRepository
    {
        Task<Hotel> AddAsync(Hotel hotel);
        Task DeleteAsync(Hotel hotel);
        Task<IEnumerable<Hotel>> GetAllAsync();
        Task<Hotel> GetByIdAsync(Guid id);
        Task UpdateAsync(Hotel hotel);
    }
}