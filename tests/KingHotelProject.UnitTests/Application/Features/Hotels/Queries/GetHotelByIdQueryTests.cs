using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.Features.Hotels.Queries;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using AutoMapper;

namespace KingHotelProject.UnitTests.Application.Features.Hotels.Queries
{
    public class GetHotelByIdQueryTests
    {
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly GetHotelByIdQueryHandler _handler;

        public GetHotelByIdQueryTests()
        {
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new GetHotelByIdQueryHandler(
                _hotelRepositoryMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithCachedData_ShouldReturnCachedResult()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var cachedHotel = new HotelResponseDto { HotelId = hotelId, HotelName = "Cached Hotel" };
            var cacheKey = $"Hotel_{hotelId}";

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<HotelResponseDto>(cacheKey))
                .ReturnsAsync(cachedHotel);

            // Act
            var result = await _handler.Handle(new GetHotelByIdQuery { Id = hotelId }, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(cachedHotel);
            _hotelRepositoryMock.Verify(r => r.GetHotelByIdAsync(hotelId), Times.Never);
        }

        [Fact]
        public async Task Handle_WithoutCachedData_ShouldFetchFromRepository()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var hotel = new Hotel { HotelId = hotelId, HotelName = "Test Hotel" };
            var expectedResult = new HotelResponseDto { HotelId = hotelId, HotelName = hotel.HotelName };
            var cacheKey = $"Hotel_{hotelId}";

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<HotelResponseDto>(cacheKey))
                .ReturnsAsync((HotelResponseDto)null);

            _hotelRepositoryMock.Setup(r => r.GetHotelByIdAsync(hotelId))
                .ReturnsAsync(hotel);

            _mapperMock.Setup(m => m.Map<HotelResponseDto>(hotel))
                .Returns(expectedResult);

            // Act
            var result = await _handler.Handle(new GetHotelByIdQuery { Id = hotelId }, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _cacheServiceMock.Verify(c => c.SetRedisCacheAsync(cacheKey, expectedResult, It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingHotel_ShouldThrowNotFoundException()
        {
            // Arrange
            var hotelId = Guid.NewGuid();

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<HotelResponseDto>($"Hotel_{hotelId}"))
                .ReturnsAsync((HotelResponseDto)null);

            _hotelRepositoryMock.Setup(r => r.GetHotelByIdAsync(hotelId))
                .ReturnsAsync((Hotel)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new GetHotelByIdQuery { Id = hotelId }, CancellationToken.None));
        }
    }
}