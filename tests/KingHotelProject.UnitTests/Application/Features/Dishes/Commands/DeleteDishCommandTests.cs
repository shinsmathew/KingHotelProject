using KingHotelProject.Application.Features.Dishes.Commands;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace KingHotelProject.UnitTests.Application.Features.Dishes.Commands
{
    public class DeleteDishCommandTests
    {
        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly DeleteDishCommandHandler _handler;

        public DeleteDishCommandTests()
        {
            _dishRepositoryMock = new Mock<IDishRepository>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new DeleteDishCommandHandler(_dishRepositoryMock.Object, _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithExistingDish_ShouldDeleteAndClearCache()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var dish = new Dish { DishId = dishId, HotelId = hotelId };

            _dishRepositoryMock.Setup(r => r.GetDishByIdAsync(dishId))
                .ReturnsAsync(dish);

            // Act
            await _handler.Handle(new DeleteDishCommand { Id = dishId }, CancellationToken.None);

            // Assert
            _dishRepositoryMock.Verify(r => r.DeleteDishAsync(dish), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync("AllDishes"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"DishesByHotel_{hotelId}"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"Dish_{dishId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistingDish_ShouldThrowNotFoundException()
        {
            // Arrange
            var dishId = Guid.NewGuid();

            _dishRepositoryMock.Setup(r => r.GetDishByIdAsync(dishId))
                .ReturnsAsync((Dish)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new DeleteDishCommand { Id = dishId }, CancellationToken.None));
        }
    }
}