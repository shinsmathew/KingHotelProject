﻿using System.ComponentModel.DataAnnotations;

namespace KingHotelProject.Application.DTOs
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

    public class HotelCreateDto
    {
        [Required(ErrorMessage = "Hotel name is required")]
        [MaxLength(100, ErrorMessage = "Hotel name cannot exceed 100 characters")]
        public string HotelName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; }

        [Required(ErrorMessage = "City is required")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string City { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        [MaxLength(20, ErrorMessage = "Zip code cannot exceed 20 characters")]
        public string Zip { get; set; }

        [Required(ErrorMessage = "Country is required")]
        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Primary phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber1 { get; set; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber2 { get; set; }

    }


    public class HotelsBulkCreateDto
    {
        [Required(ErrorMessage = "At least one hotel is required")]
        [MaxLength(50, ErrorMessage = "Cannot create more than 50 hotels at once")]
        public List<HotelCreateDto> Hotels { get; set; } = new();
    }
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