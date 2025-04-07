namespace KingHotelProject.Application.DTOs
{
    public class DishResponseDto
    {
        public Guid DishId { get; set; }
        public string DishName { get; set; }
        public int Price { get; set; }
        public Guid HotelId { get; set; }
    }

    public class DishCreateDto
    {
        public string DishName { get; set; }
        public int Price { get; set; }
        public Guid HotelId { get; set; }
    }

    public class DishesBulkCreateDto
    {
        public List<DishCreateDto> Dishes { get; set; } = new List<DishCreateDto>();
    }

    public class DishUpdateDto
    {
        public string DishName { get; set; }
        public int Price { get; set; }
    }
}