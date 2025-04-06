
using KingHotelProject.Core.Entities;
using System.Threading.Tasks;

namespace KingHotelProject.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<User> GetByUserNameAsync(string userName);
        Task<User> GetByEmailAsync(string email);
    }
}