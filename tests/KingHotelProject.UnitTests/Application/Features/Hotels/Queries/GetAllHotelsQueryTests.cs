using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.Features.Hotels.Queries;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using AutoMapper;
using KingHotelProject.Application.Mappings;

namespace KingHotelProject.UnitTests.Application.Features.Hotels.Queries
{
    public class GetAllHotelsQueryTests
    {
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly GetAllHotelsQueryHandler _handler;

        public GetAllHotelsQueryTests()
        {
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new GetAllHotelsQueryHandler(
                _hotelRepositoryMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithCachedData_ShouldReturnCachedResult()
        {
            // Arrange
            var cachedHotels = new List<HotelResponseDto>
            {
                new HotelResponseDto { HotelId = Guid.NewGuid(), HotelName = "Cached Hotel" }
            };

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<IEnumerable<HotelResponseDto>>("AllHotels"))
                .ReturnsAsync(cachedHotels);

            // Act
            var result = await _handler.Handle(new GetAllHotelsQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(cachedHotels);
            _hotelRepositoryMock.Verify(r => r.GetAllHotelAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WithoutCachedData_ShouldFetchFromRepository()
        {
            // Arrange
            var hotels = new List<Hotel>
            {
                new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel" }
            };

            var expectedResult = new List<HotelResponseDto>
            {
                new HotelResponseDto { HotelId = hotels[0].HotelId, HotelName = hotels[0].HotelName }
            };

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<IEnumerable<HotelResponseDto>>("AllHotels"))
                .ReturnsAsync((IEnumerable<HotelResponseDto>)null);

            _hotelRepositoryMock.Setup(r => r.GetAllHotelAsync())
                .ReturnsAsync(hotels);

            _mapperMock.Setup(m => m.Map<IEnumerable<HotelResponseDto>>(hotels))
                .Returns(expectedResult);

            // Act
            var result = await _handler.Handle(new GetAllHotelsQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _cacheServiceMock.Verify(c => c.SetRedisCacheAsync("AllHotels", expectedResult, It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}