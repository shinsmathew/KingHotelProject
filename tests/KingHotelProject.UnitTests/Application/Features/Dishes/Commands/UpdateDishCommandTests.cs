using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.Features.Dishes.Commands;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using AutoMapper;

namespace KingHotelProject.UnitTests.Application.Features.Dishes.Commands
{
    public class UpdateDishCommandTests
    {
        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IValidator<DishUpdateDto>> _validatorMock;
        private readonly UpdateDishCommandHandler _handler;

        public UpdateDishCommandTests()
        {
            _dishRepositoryMock = new Mock<IDishRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
            _validatorMock = new Mock<IValidator<DishUpdateDto>>();
            _handler = new UpdateDishCommandHandler(
                _dishRepositoryMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldUpdateAndClearCache()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var hotelId = Guid.NewGuid();
            var dish = new Dish { DishId = dishId, HotelId = hotelId };
            var updateDto = new DishUpdateDto { DishName = "Updated Dish", Price = 15.99m };

            _validatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _dishRepositoryMock.Setup(r => r.GetDishByIdAsync(dishId))
                .ReturnsAsync(dish);

            // Act
            await _handler.Handle(new UpdateDishCommand { Id = dishId, DishUpdateDto = updateDto }, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map(updateDto, dish), Times.Once);
            _dishRepositoryMock.Verify(r => r.UpdateDishAsync(dish), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync("AllDishes"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"DishesByHotel_{hotelId}"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"Dish_{dishId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var updateDto = new DishUpdateDto();
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Name", "Error") };

            _validatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(new UpdateDishCommand { Id = dishId, DishUpdateDto = updateDto }, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithNonExistingDish_ShouldThrowNotFoundException()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var updateDto = new DishUpdateDto { DishName = "Updated Dish" };

            _validatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _dishRepositoryMock.Setup(r => r.GetDishByIdAsync(dishId))
                .ReturnsAsync((Dish)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new UpdateDishCommand { Id = dishId, DishUpdateDto = updateDto }, CancellationToken.None));
        }
    }
}