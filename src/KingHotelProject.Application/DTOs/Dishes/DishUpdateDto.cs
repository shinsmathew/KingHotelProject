using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingHotelProject.Application.DTOs.Dishes
{
    public class DishUpdateDto
    {
        [Required(ErrorMessage = "Dish name is required")]
        [MaxLength(50, ErrorMessage = "Dish name cannot exceed 50 characters")]
        public string DishName { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
