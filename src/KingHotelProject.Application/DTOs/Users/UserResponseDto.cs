using KingHotelProject.Core.Enums;

namespace KingHotelProject.Application.DTOs.Users
{
    public class UserResponseDto
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public UserRole Role { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
