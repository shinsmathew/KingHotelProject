using KingHotelProject.Core.Entities;
using KingHotelProject.Infrastructure.Data;
using KingHotelProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using System;

namespace KingHotelProject.UnitTests.Infrastructure.Repositories
{
    public class DishRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly DishRepository _repository;

        public DishRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new DishRepository(_context);

            // Seed test data
            var hotel = new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel" };
            _context.Hotels.Add(hotel);
            _context.Dishes.AddRange(
                new Dish { DishId = Guid.NewGuid(), DishName = "Dish 1", HotelId = hotel.HotelId },
                new Dish { DishId = Guid.NewGuid(), DishName = "Dish 2", HotelId = hotel.HotelId }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllDishAsync_ShouldReturnAllDishes()
        {
            // Act
            var result = await _repository.GetAllDishAsync();

            // Assert
            result.Should().HaveCount(2);
            result.All(d => d.Hotel != null).Should().BeTrue();
        }

        [Fact]
        public async Task GetDishByIdAsync_WithExistingId_ShouldReturnDish()
        {
            // Arrange
            var dish = _context.Dishes.First();

            // Act
            var result = await _repository.GetDishByIdAsync(dish.DishId);

            // Assert
            result.Should().NotBeNull();
            result.DishId.Should().Be(dish.DishId);
            result.Hotel.Should().NotBeNull();
        }

        [Fact]
        public async Task GetDishByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetDishByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddDishAsync_ShouldAddDish()
        {
            // Arrange
            var hotelId = _context.Hotels.First().HotelId;
            var dish = new Dish { DishName = "New Dish", HotelId = hotelId };

            // Act
            var result = await _repository.AddDishAsync(dish);

            // Assert
            result.Should().NotBeNull();
            result.DishId.Should().NotBe(Guid.Empty);
            _context.Dishes.Should().HaveCount(3);
        }

        [Fact]
        public async Task UpdateDishAsync_ShouldUpdateDish()
        {
            // Arrange
            var dish = _context.Dishes.First();
            var newName = "Updated Dish";
            dish.DishName = newName;

            // Act
            await _repository.UpdateDishAsync(dish);

            // Assert
            var updatedDish = await _repository.GetDishByIdAsync(dish.DishId);
            updatedDish.DishName.Should().Be(newName);
        }

        [Fact]
        public async Task DeleteDishAsync_ShouldRemoveDish()
        {
            // Arrange
            var dish = _context.Dishes.First();

            // Act
            await _repository.DeleteDishAsync(dish);

            // Assert
            _context.Dishes.Should().HaveCount(1);
            (await _repository.GetDishByIdAsync(dish.DishId)).Should().BeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}