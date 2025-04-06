
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

        public async Task<IEnumerable<Dish>> GetAllAsync()
        {
            return await _context.Dishes.Include(d => d.Hotel).ToListAsync();
        }

        public async Task<Dish> GetByIdAsync(Guid id)
        {
            return await _context.Dishes.Include(d => d.Hotel).FirstOrDefaultAsync(d => d.DishId == id);
        }

        public async Task<IEnumerable<Dish>> GetByHotelIdAsync(Guid hotelId)
        {
            return await _context.Dishes
                .Where(d => d.HotelId == hotelId)
                .Include(d => d.Hotel)
                .ToListAsync();
        }

        public async Task<Dish> AddAsync(Dish dish)
        {
            await _context.Dishes.AddAsync(dish);
            await _context.SaveChangesAsync();
            return dish;
        }
      
        public async Task UpdateAsync(Dish dish)
        {
            _context.Dishes.Update(dish);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Dish dish)
        {
            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();
        }
    }
}