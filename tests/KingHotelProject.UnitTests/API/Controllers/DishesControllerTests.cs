using KingHotelProject.API.Controllers;
using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.Features.Dishes.Commands;
using KingHotelProject.Application.Features.Dishes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System;

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
    }
}