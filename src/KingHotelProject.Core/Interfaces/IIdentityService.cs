using KingHotelProject.Core.Entities;

namespace KingHotelProject.Core.Interfaces
{
    public interface IIdentityService
    {
        string GenerateJwtToken(User user);
        string HashPassword(string password);
        bool VerifyPassword(string hashedPassword, string providedPassword);
    }
}