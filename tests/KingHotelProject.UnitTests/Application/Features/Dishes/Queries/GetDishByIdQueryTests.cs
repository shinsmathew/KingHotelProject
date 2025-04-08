using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.Features.Dishes.Queries;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using AutoMapper;

namespace KingHotelProject.UnitTests.Application.Features.Dishes.Queries
{
    public class GetDishByIdQueryTests
    {
        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly GetDishByIdQueryHandler _handler;

        public GetDishByIdQueryTests()
        {
            _dishRepositoryMock = new Mock<IDishRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new GetDishByIdQueryHandler(
                _dishRepositoryMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithCachedData_ShouldReturnCachedResult()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var cachedDish = new DishResponseDto { DishId = dishId, DishName = "Cached Dish" };
            var cacheKey = $"Dish_{dishId}";

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<DishResponseDto>(cacheKey))
                .ReturnsAsync(cachedDish);

            // Act
            var result = await _handler.Handle(new GetDishByIdQuery { Id = dishId }, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(cachedDish);
            _dishRepositoryMock.Verify(r => r.GetDishByIdAsync(dishId), Times.Never);
        }

        [Fact]
        public async Task Handle_WithoutCachedData_ShouldFetchFromRepository()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var dish = new Dish { DishId = dishId, DishName = "Test Dish" };
            var expectedResult = new DishResponseDto { DishId = dishId, DishName = dish.DishName };
            var cacheKey = $"Dish_{dishId}";

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<DishResponseDto>(cacheKey))
                .ReturnsAsync((DishResponseDto)null);

            _dishRepositoryMock.Setup(r => r.GetDishByIdAsync(dishId))
                .ReturnsAsync(dish);

            _mapperMock.Setup(m => m.Map<DishResponseDto>(dish))
                .Returns(expectedResult);

            // Act
            var result = await _handler.Handle(new GetDishByIdQuery { Id = dishId }, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expectedResult);
            _cacheServiceMock.Verify(c => c.SetRedisCacheAsync(cacheKey, expectedResult, It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingDish_ShouldThrowNotFoundException()
        {
            // Arrange
            var dishId = Guid.NewGuid();

            _cacheServiceMock.Setup(c => c.GetRedisCacheAsync<DishResponseDto>($"Dish_{dishId}"))
                .ReturnsAsync((DishResponseDto)null);

            _dishRepositoryMock.Setup(r => r.GetDishByIdAsync(dishId))
                .ReturnsAsync((Dish)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new GetDishByIdQuery { Id = dishId }, CancellationToken.None));
        }
    }
}