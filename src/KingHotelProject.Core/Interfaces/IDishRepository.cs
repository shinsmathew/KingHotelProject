using KingHotelProject.Core.Entities;

namespace KingHotelProject.Core.Interfaces
{
    public interface IDishRepository
    {
        Task<IEnumerable<Dish>> GetAllDishAsync();
        Task<Dish> GetDishByIdAsync(Guid id);
        Task<Dish> AddDishAsync(Dish dish);
        Task UpdateDishAsync(Dish dish);
        Task DeleteDishAsync(Dish dish);
    }
}