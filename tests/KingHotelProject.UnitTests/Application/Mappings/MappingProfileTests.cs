using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.DTOs.Users;
using KingHotelProject.Application.Mappings;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Enums;
using AutoMapper;
using Xunit;
using FluentAssertions;
using System;

namespace KingHotelProject.UnitTests.Application.Mappings
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = configuration.CreateMapper();
        }

        [Fact]
        public void MappingProfile_ShouldMapUserRegisterDtoToUser()
        {
            // Arrange
            var dto = new UserRegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123!",
                Role = 0
            };

            // Act
            var user = _mapper.Map<User>(dto);

            // Assert
            user.FirstName.Should().Be(dto.FirstName);
            user.LastName.Should().Be(dto.LastName);
            user.Email.Should().Be(dto.Email);
            user.UserName.Should().Be(dto.UserName);
            user.Role.Should().Be(UserRole.Admin);
            user.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void MappingProfile_ShouldMapHotelCreateDtoToHotel()
        {
            // Arrange
            var dto = new HotelCreateDto
            {
                HotelName = "Test Hotel",
                Address = "123 Test St",
                City = "Test City",
                Zip = "12345",
                Country = "Test Country",
                Email = "test@example.com",
                PhoneNumber1 = "+1234567890"
            };

            // Act
            var hotel = _mapper.Map<Hotel>(dto);

            // Assert
            hotel.HotelName.Should().Be(dto.HotelName);
            hotel.Address.Should().Be(dto.Address);
            hotel.City.Should().Be(dto.City);
            hotel.Zip.Should().Be(dto.Zip);
            hotel.Country.Should().Be(dto.Country);
            hotel.Email.Should().Be(dto.Email);
            hotel.PhoneNumber1.Should().Be(dto.PhoneNumber1);
            hotel.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void MappingProfile_ShouldMapDishToDishResponseDto()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = Guid.NewGuid(),
                DishName = "Test Dish",
                Price = 10.99m,
                HotelId = Guid.NewGuid()
            };

            // Act
            var dto = _mapper.Map<DishResponseDto>(dish);

            // Assert
            dto.DishId.Should().Be(dish.DishId);
            dto.DishName.Should().Be(dish.DishName);
            dto.Price.Should().Be(dish.Price);
            dto.HotelId.Should().Be(dish.HotelId);
        }
    }
}