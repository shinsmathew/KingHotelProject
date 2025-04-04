using KingHotelProject.Core.Entities;
using System.Threading.Tasks;

namespace KingHotelProject.Core.Interfaces
{
    public interface IIdentityService
    {
        string GenerateJwtToken(User user);
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);

        // Additional useful methods you might want:
        Task<User> GetCurrentUserAsync();
        string GetCurrentUserId();
        Task<bool> IsInRoleAsync(string userId, string role);
    }
}