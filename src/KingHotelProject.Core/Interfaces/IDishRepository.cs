using KingHotelProject.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KingHotelProject.Core.Interfaces
{
    public interface IDishRepository
    {
        Task<Dish> AddAsync(Dish dish);
        Task DeleteAsync(Dish dish);
        Task<IEnumerable<Dish>> GetAllAsync();
        Task<Dish> GetByIdAsync(Guid id);
        Task<IEnumerable<Dish>> GetByHotelIdAsync(Guid hotelId);
        Task UpdateAsync(Dish dish);
    }
}