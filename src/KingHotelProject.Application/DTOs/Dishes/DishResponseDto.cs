namespace KingHotelProject.Application.DTOs.Dishes
{
    public class DishResponseDto
    {
        public Guid DishId { get; set; }
        public string DishName { get; set; }
        public decimal Price { get; set; }
        public Guid HotelId { get; set; }
    }
}
