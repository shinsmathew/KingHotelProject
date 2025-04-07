using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KingHotelProject.Infrastructure.Repositories
{
    public class DishRepository : IDishRepository
    {
        private readonly ApplicationDbContext _context;

        public DishRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Dish>> GetAllDishAsync()
        {
            return await _context.Dishes
                .AsNoTracking()
                .Include(d => d.Hotel)
                .ToListAsync();
        }

        public async Task<Dish> GetDishByIdAsync(Guid id)
        {
            return await _context.Dishes
                .AsNoTracking()
                .Include(d => d.Hotel)
                .FirstOrDefaultAsync(d => d.DishId == id);
        }

        public async Task<Dish> AddDishAsync(Dish dish)
        {
            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();
            return dish;
        }

        public async Task UpdateDishAsync(Dish dish)
        {
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDishAsync(Dish dish)
        {
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
        }
    }
}