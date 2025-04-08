using KingHotelProject.Core.Entities;
using KingHotelProject.Infrastructure.Data;
using KingHotelProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using System.Linq;
using System;
using System.Collections.Generic;

namespace KingHotelProject.UnitTests.Infrastructure.Repositories
{
    public class HotelRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly HotelRepository _repository;

        public HotelRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new HotelRepository(_context);

            // Seed test data
            _context.Hotels.AddRange(
                new Hotel { HotelId = Guid.NewGuid(), HotelName = "Hotel 1" },
                new Hotel { HotelId = Guid.NewGuid(), HotelName = "Hotel 2" }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllHotelAsync_ShouldReturnAllHotels()
        {
            // Act
            var result = await _repository.GetAllHotelAsync();

            // Assert
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetHotelByIdAsync_WithExistingId_ShouldReturnHotel()
        {
            // Arrange
            var hotel = _context.Hotels.First();

            // Act
            var result = await _repository.GetHotelByIdAsync(hotel.HotelId);

            // Assert
            result.Should().NotBeNull();
            result.HotelId.Should().Be(hotel.HotelId);
        }

        [Fact]
        public async Task GetHotelByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetHotelByIdAsync(Guid.NewGuid());

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetHotelByIdsAsync_ShouldReturnMatchingHotels()
        {
            // Arrange
            var hotelIds = _context.Hotels.Select(h => h.HotelId).Take(1).ToList();

            // Act
            var result = await _repository.GetHotelByIdsAsync(hotelIds);

            // Assert
            result.Should().HaveCount(1);
            result.First().HotelId.Should().Be(hotelIds.First());
        }

        [Fact]
        public async Task AddHotelAsync_ShouldAddHotel()
        {
            // Arrange
            var hotel = new Hotel { HotelName = "New Hotel" };

            // Act
            var result = await _repository.AddHotelAsync(hotel);

            // Assert
            result.Should().NotBeNull();
            result.HotelId.Should().NotBe(Guid.Empty);
            _context.Hotels.Should().HaveCount(3);
        }

        [Fact]
        public async Task UpdateHotelAsync_ShouldUpdateHotel()
        {
            // Arrange
            var hotel = _context.Hotels.First();
            var newName = "Updated Hotel";
            hotel.HotelName = newName;

            // Act
            await _repository.UpdateHotelAsync(hotel);

            // Assert
            var updatedHotel = await _repository.GetHotelByIdAsync(hotel.HotelId);
            updatedHotel.HotelName.Should().Be(newName);
        }

        [Fact]
        public async Task DeleteHotelAsync_ShouldRemoveHotel()
        {
            // Arrange
            var hotel = _context.Hotels.First();

            // Act
            await _repository.DeleteHotelAsync(hotel);

            // Assert
            _context.Hotels.Should().HaveCount(1);
            (await _repository.GetHotelByIdAsync(hotel.HotelId)).Should().BeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}