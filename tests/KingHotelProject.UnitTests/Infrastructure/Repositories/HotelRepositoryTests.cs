using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Data;
using Moq;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KingHotelProject.Infrastructure.Repositories;

namespace KingHotelProject.UnitTests.Infrastructure.Repositories
{
    public class HotelRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly HotelRepository _repository;

        public HotelRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            _repository = new HotelRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetAllHotelAsync_ShouldReturnAllHotels()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel 1" },
                new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel 2" }
            };

            _contextMock.Setup(c => c.Hotels)
                .ReturnsDbSet(hotels);

            // Act
            var result = await _repository.GetAllHotelAsync();

            // Assert
            result.Should().BeEquivalentTo(hotels);
        }

        [Fact]
        public async Task GetHotelByIdAsync_WithExistingId_ShouldReturnHotel()
        {
            // Arrange
            var hotel = new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel" };

            _contextMock.Setup(c => c.Hotels)
                .ReturnsDbSet(new List<Hotel> { hotel });

            // Act
            var result = await _repository.GetHotelByIdAsync(hotel.HotelId);

            // Assert
            result.Should().BeEquivalentTo(hotel);
        }

        [Fact]
        public async Task GetHotelByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            var hotelId = Guid.NewGuid();

            _contextMock.Setup(c => c.Hotels)
                .ReturnsDbSet(new List<Hotel>());

            // Act
            var result = await _repository.GetHotelByIdAsync(hotelId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetHotelByIdsAsync_ShouldReturnHotels()
        {
            // Arrange
            var hotel1 = new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel 1" };
            var hotel2 = new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel 2" };

            var hotels = new List<Hotel> { hotel1, hotel2 };

            _contextMock.Setup(c => c.Hotels)
                .ReturnsDbSet(hotels);

            // Act
            var result = await _repository.GetHotelByIdsAsync(new List<Guid> { hotel1.HotelId, hotel2.HotelId });

            // Assert
            result.Should().BeEquivalentTo(new List<Hotel> { hotel1, hotel2 });
        }

        [Fact]
        public async Task AddHotelAsync_ShouldAddHotel()
        {
            // Arrange
            var hotel = new Hotel { HotelName = "New Hotel" };

            _contextMock.Setup(c => c.Hotels.AddAsync(hotel, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.AddHotelAsync(hotel);

            // Assert
            _contextMock.Verify(c => c.Hotels.AddAsync(hotel, It.IsAny<CancellationToken>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateHotelAsync_ShouldUpdateHotel()
        {
            // Arrange
            var hotel = new Hotel { HotelId = Guid.NewGuid(), HotelName = "Updated Hotel" };

            _contextMock.Setup(c => c.Hotels.Update(hotel))
                .Returns(_contextMock.Object.Entry(hotel));

            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.UpdateHotelAsync(hotel);

            // Assert
            _contextMock.Verify(c => c.Hotels.Update(hotel), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteHotelAsync_ShouldDeleteHotel()
        {
            // Arrange
            var hotel = new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel" };

            _contextMock.Setup(c => c.Hotels.Remove(hotel))
                .Returns(_contextMock.Object.Entry(hotel));

            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.DeleteHotelAsync(hotel);

            // Assert
            _contextMock.Verify(c => c.Hotels.Remove(hotel), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }

    public static class DbSetMockExtensions
    {
        public static DbSet<T> ReturnsDbSet<T>(this Mock<ApplicationDbContext> mockContext, List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            dbSet.Setup(d => d.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Callback<T, CancellationToken>((entity, token) => sourceList.Add(entity))
                .Returns(Task.CompletedTask);

            dbSet.Setup(d => d.Remove(It.IsAny<T>()))
                .Callback<T>(entity => sourceList.Remove(entity));

            dbSet.Setup(d => d.Update(It.IsAny<T>()))
                .Callback<T>(entity => sourceList[sourceList.IndexOf(entity)] = entity);

            mockContext.Setup(c => c.Set<T>()).Returns(dbSet.Object);

            return dbSet.Object;
        }
    }
}