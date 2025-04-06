using KingHotelProject.Core.Entities;

namespace KingHotelProject.Core.Interfaces
{
    public interface IDishRepository
    {
        Task<IEnumerable<Dish>> GetAllAsync();
        Task<Dish> GetByIdAsync(Guid id);
        Task<IEnumerable<Dish>> GetByHotelIdAsync(Guid hotelId);
        Task<Dish> AddAsync(Dish dish);
        Task UpdateAsync(Dish dish);
        Task DeleteAsync(Dish dish);
    }
}