using System.ComponentModel.DataAnnotations;


namespace KingHotelProject.Application.DTOs.Hotels
{
    public class HotelUpdateDto
    {
        [MaxLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters")]
        public string? HotelName { get; set; }

        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string? City { get; set; }

        [MaxLength(20, ErrorMessage = "Zip code cannot exceed 20 characters")]
        public string? Zip { get; set; }

        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string? Country { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber1 { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber2 { get; set; }
    }
}
