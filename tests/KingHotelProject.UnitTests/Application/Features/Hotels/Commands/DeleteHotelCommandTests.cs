using KingHotelProject.Application.Features.Hotels.Commands;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace KingHotelProject.UnitTests.Application.Features.Hotels.Commands
{
    public class DeleteHotelCommandTests
    {
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly DeleteHotelCommandHandler _handler;

        public DeleteHotelCommandTests()
        {
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new DeleteHotelCommandHandler(
                _hotelRepositoryMock.Object,
                _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithExistingHotel_ShouldDeleteAndClearCache()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var hotel = new Hotel { HotelId = hotelId };

            _hotelRepositoryMock.Setup(r => r.GetHotelByIdAsync(hotelId))
                .ReturnsAsync(hotel);

            // Act
            await _handler.Handle(new DeleteHotelCommand { Id = hotelId }, CancellationToken.None);

            // Assert
            _hotelRepositoryMock.Verify(r => r.DeleteHotelAsync(hotel), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync("AllHotels"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"Hotel_{hotelId}"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"DishesByHotel_{hotelId}"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync("AllDishes"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingHotel_ShouldThrowNotFoundException()
        {
            // Arrange
            var hotelId = Guid.NewGuid();

            _hotelRepositoryMock.Setup(r => r.GetHotelByIdAsync(hotelId))
                .ReturnsAsync((Hotel)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new DeleteHotelCommand { Id = hotelId }, CancellationToken.None));
        }
    }
}