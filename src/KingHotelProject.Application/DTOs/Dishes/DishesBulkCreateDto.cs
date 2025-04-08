
using System.ComponentModel.DataAnnotations;

namespace KingHotelProject.Application.DTOs.Dishes
{
    public class DishCreateDto
    {
        [Required(ErrorMessage = "Dish name is required")]
        [MaxLength(50, ErrorMessage = "Dish name cannot exceed 50 characters")]
        public string DishName { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Hotel ID is required")]
        public Guid HotelId { get; set; }
    }

    public class DishesBulkCreateDto
    {
        [Required(ErrorMessage = "At least one dish is required")]
        [MaxLength(100, ErrorMessage = "Cannot create more than 100 dishes at once")]
        public List<DishCreateDto> Dishes { get; set; } = new();
    }
}
