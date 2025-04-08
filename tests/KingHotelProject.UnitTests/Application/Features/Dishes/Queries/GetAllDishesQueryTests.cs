using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.Features.Dishes.Queries;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using Moq;
using FluentAssertions;
using AutoMapper;


namespace KingHotelProject.UnitTests.Application.Features.Dishes.Queries
{
    public class GetAllDishesQueryTests
    {
        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly GetAllDishesQueryHandler _handler;

        public GetAllDishesQueryTests()
        {
            _dishRepositoryMock = new Mock<IDishRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new GetAllDishesQueryHandler(
                _dishRepositoryMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithCachedData_ShouldReturnCachedResult()
        {
            // Arrange
            var cachedDishes = new List<DishResponseDto>
            {
                new DishResponseDto { DishId = Guid.NewGuid(), DishName = "Cached Dish" }
            };

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<IEnumerable<DishResponseDto>>("AllDishes"))
                .ReturnsAsync(cachedDishes);

            // Act
            var result = await _handler.Handle(new GetAllDishesQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(cachedDishes);
            _dishRepositoryMock.Verify(r => r.GetAllDishAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_WithoutCachedData_ShouldFetchFromRepository()
        {
            // Arrange
            var dishes = new List<Dish>
            {
                new Dish { DishId = Guid.NewGuid(), DishName = "Test Dish" }
            };

            var expectedResult = new List<DishResponseDto>
            {
                new DishResponseDto { DishId = dishes[0].DishId, DishName = dishes[0].DishName }
            };

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<IEnumerable<DishResponseDto>>("AllDishes"))
                .ReturnsAsync((IEnumerable<DishResponseDto>)null);

            _dishRepositoryMock.Setup(r => r.GetAllDishAsync())
                .ReturnsAsync(dishes);

            _mapperMock.Setup(m => m.Map<IEnumerable<DishResponseDto>>(dishes))
                .Returns(expectedResult);

            // Act
            var result = await _handler.Handle(new GetAllDishesQuery(), CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _cacheServiceMock.Verify(c => c.SetRedisCacheAsync("AllDishes", expectedResult, It.IsAny<TimeSpan>()), Times.Once);
        }
    }
}