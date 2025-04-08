using KingHotelProject.Application.Mappings;
using AutoMapper;
using Xunit;
using KingHotelProject.Core.Entities;
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.DTOs.Users;
using FluentAssertions;
using KingHotelProject.Core.Enums;

namespace KingHotelProject.UnitTests.Application.Mappings
{
    public class MappingProfileTests
    {
        private readonly IMapper _mapper;

        public MappingProfileTests()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });

            _mapper = mappingConfig.CreateMapper();
        }

        [Fact]
        public void ShouldHaveValidConfiguration()
        {
            // Act & Assert
            _mapper.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [Fact]
        public void ShouldMap_Hotel_To_HotelResponseDto()
        {
            // Arrange
            var hotel = new Hotel
            {
                HotelId = Guid.NewGuid(),
                HotelName = "Test Hotel",
                Address = "123 Test St",
                City = "Test City",
                Zip = "12345",
                Country = "Test Country",
                Email = "test@example.com",
                PhoneNumber1 = "123-456-7890",
                CreatedDate = DateTime.UtcNow
            };

            // Act
            var result = _mapper.Map<HotelResponseDto>(hotel);

            // Assert
            result.Should().NotBeNull();
            result.HotelId.Should().Be(hotel.HotelId);
            result.HotelName.Should().Be(hotel.HotelName);
            result.Address.Should().Be(hotel.Address);
            result.City.Should().Be(hotel.City);
            result.Zip.Should().Be(hotel.Zip);
            result.Country.Should().Be(hotel.Country);
            result.Email.Should().Be(hotel.Email);
            result.PhoneNumber1.Should().Be(hotel.PhoneNumber1);
            result.CreatedDate.Should().Be(hotel.CreatedDate);
        }

        [Fact]
        public void ShouldMap_Dish_To_DishResponseDto()
        {
            // Arrange
            var dish = new Dish
            {
                DishId = Guid.NewGuid(),
                DishName = "Test Dish",
                Price = 10.99m,
                HotelId = Guid.NewGuid(),
                CreatedDate = DateTime.UtcNow
            };

            // Act
            var result = _mapper.Map<DishResponseDto>(dish);

            // Assert
            result.Should().NotBeNull();
            result.DishId.Should().Be(dish.DishId);
            result.DishName.Should().Be(dish.DishName);
            result.Price.Should().Be(dish.Price);
            result.HotelId.Should().Be(dish.HotelId);
            result.CreatedDate.Should().Be(dish.CreatedDate);
        }

        [Fact]
        public void ShouldMap_User_To_UserResponseDto()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                UserName = "testuser",
                Role = UserRole.Admin,
                CreatedDate = DateTime.UtcNow
            };

            // Act
            var result = _mapper.Map<UserResponseDto>(user);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().Be(user.UserId);
            result.FirstName.Should().Be(user.FirstName);
            result.LastName.Should().Be(user.LastName);
            result.Email.Should().Be(user.Email);
            result.UserName.Should().Be(user.UserName);
            result.Role.Should().Be(user.Role);
            result.CreatedDate.Should().Be(user.CreatedDate);
        }
    }
}