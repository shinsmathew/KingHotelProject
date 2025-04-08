using KingHotelProject.Application.DTOs.Dishes;

namespace KingHotelProject.Application.DTOs.Hotels
{
    public class HotelResponseDto
    {
        public Guid HotelId { get; set; }
        public string HotelName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string PhoneNumber1 { get; set; }
        public string PhoneNumber2 { get; set; }
        public DateTime CreatedDate { get; set; }
        public ICollection<DishResponseDto> Dishes { get; set; }
    }
}
