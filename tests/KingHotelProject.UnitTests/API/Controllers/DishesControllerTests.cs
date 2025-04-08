using KingHotelProject.API.Controllers;
using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.Features.Dishes.Commands;
using KingHotelProject.Application.Features.Dishes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using FluentValidation;
using FluentValidation.Results;


namespace KingHotelProject.UnitTests.API.Controllers
{
    public class DishesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DishesController _controller;

        public DishesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new DishesController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAllDishes_ReturnsOkResultWithDishes()
        {
            // Arrange
            var dishes = new List<DishResponseDto>
            {
                new DishResponseDto { DishId = Guid.NewGuid(), DishName = "Test Dish 1" },
                new DishResponseDto { DishId = Guid.NewGuid(), DishName = "Test Dish 2" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllDishesQuery>(), default))
                .ReturnsAsync(dishes);

            // Act
            var result = await _controller.GetAllDishes();

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<DishResponseDto>>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(dishes);
        }

        [Fact]
        public async Task GetDishesById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var dish = new DishResponseDto { DishId = dishId, DishName = "Test Dish" };

            _mediatorMock.Setup(m => m.Send(It.Is<GetDishByIdQuery>(q => q.Id == dishId), default))
                .ReturnsAsync(dish);

            // Act
            var result = await _controller.GetDishesById(dishId);

            // Assert
            result.Should().BeOfType<ActionResult<DishResponseDto>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(dish);
        }

        [Fact]
        public async Task CreateDishesBulk_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var dishesDto = new DishesBulkCreateDto
            {
                Dishes = new List<DishCreateDto>
                {
                    new DishCreateDto { DishName = "Test Dish", Price = 10.99m, HotelId = Guid.NewGuid() }
                }
            };

            var createdDishes = new List<DishResponseDto>
            {
                new DishResponseDto { DishId = Guid.NewGuid(), DishName = "Test Dish" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateDishesBulkCommand>(), default))
                .ReturnsAsync(createdDishes);

            // Act
            var result = await _controller.CreateDishesBulk(dishesDto);

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<DishResponseDto>>>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Value.Should().BeEquivalentTo(createdDishes);
            createdResult.ActionName.Should().Be(nameof(_controller.GetAllDishes));
        }

        [Fact]
        public async Task UpdateDishes_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var dishId = Guid.NewGuid();
            var dishUpdateDto = new DishUpdateDto
            {
                DishName = "Updated Dish",
                Price = 15.99m
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateDishCommand>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateDishes(dishId, dishUpdateDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateDishCommand>(c =>
                c.Id == dishId && c.DishUpdateDto == dishUpdateDto), default), Times.Once);
        }

        [Fact]
        public async Task DeleteDishes_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var dishId = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteDishCommand>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteDishes(dishId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteDishCommand>(c =>
                c.Id == dishId), default), Times.Once);
        }

    }
}