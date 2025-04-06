using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KingHotelProject.Infrastructure.Repositories
{
    public class HotelRepository : IHotelRepository
    {
        private readonly ApplicationDbContext _context;

        public HotelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Hotel>> GetAllHotelAsync()
        {
            return await _context.Hotels
                .AsNoTracking()
                .Include(h => h.Dishes)
                .ToListAsync();
        }

        public async Task<Hotel> GetHotelByIdAsync(Guid id)
        {
            return await _context.Hotels
                 .AsNoTracking()
                .Include(h => h.Dishes)
                .FirstOrDefaultAsync(h => h.HotelId == id);
        }

        public async Task<Hotel> AddHotelAsync(Hotel hotel)
        {
            await _context.Hotels.AddAsync(hotel);
            await _context.SaveChangesAsync();
            return hotel;
        }

        public async Task UpdateHotelAsync(Hotel hotel)
        {
            _context.Hotels.Update(hotel);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteHotelAsync(Hotel hotel)
        {
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();
        }
    }
}