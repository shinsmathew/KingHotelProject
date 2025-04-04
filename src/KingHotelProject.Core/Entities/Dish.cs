using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KingHotelProject.Core.Entities
{
    public class Dish
    {
        [Key]
        public Guid DishId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        public string DishName { get; set; }

        [Required]
        public int Price { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public Guid HotelId { get; set; }

        public Hotel Hotel { get; set; }
    }
}
