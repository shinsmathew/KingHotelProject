namespace KingHotelProject.Application.DTOs.Users
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
        public UserResponseDto User { get; set; }
    }
}
