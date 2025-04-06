using KingHotelProject.Core.Enums;

namespace KingHotelProject.Application.DTOs
{
    public class UserRegisterDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
    }

    public class UserLoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

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

    public class AuthResponseDto
    {
        public string Token { get; set; }
        public UserResponseDto User { get; set; }
    }
}