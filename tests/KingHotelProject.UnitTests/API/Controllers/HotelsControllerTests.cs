using KingHotelProject.API.Controllers;
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.Features.Hotels.Commands;
using KingHotelProject.Application.Features.Hotels.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FluentAssertions;
using System.Collections.Generic;
using System;

namespace KingHotelProject.UnitTests.API.Controllers
{
    public class HotelsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly HotelsController _controller;

        public HotelsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new HotelsController(_mediatorMock.Object);
        }

        [Fact]
        public async Task GetAllHotelData_ReturnsOkResultWithHotels()
        {
            // Arrange
            var hotels = new List<HotelResponseDto>
            {
                new HotelResponseDto { HotelId = Guid.NewGuid(), HotelName = "Test Hotel 1" },
                new HotelResponseDto { HotelId = Guid.NewGuid(), HotelName = "Test Hotel 2" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllHotelsQuery>(), default))
                .ReturnsAsync(hotels);

            // Act
            var result = await _controller.GetAllHotelData();

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<HotelResponseDto>>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(hotels);
        }

        [Fact]
        public async Task GetHotalDataById_WithValidId_ReturnsOkResult()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var hotel = new HotelResponseDto { HotelId = hotelId, HotelName = "Test Hotel" };

            _mediatorMock.Setup(m => m.Send(It.Is<GetHotelByIdQuery>(q => q.Id == hotelId), default))
                .ReturnsAsync(hotel);

            // Act
            var result = await _controller.GetHotalDataById(hotelId);

            // Assert
            result.Should().BeOfType<ActionResult<HotelResponseDto>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(hotel);
        }

        [Fact]
        public async Task CreateHotelsBulk_WithValidData_ReturnsCreatedResult()
        {
            // Arrange
            var hotelsDto = new HotelsBulkCreateDto
            {
                Hotels = new List<HotelCreateDto>
                {
                    new HotelCreateDto { HotelName = "Test Hotel", Address = "123 Test St" }
                }
            };

            var createdHotels = new List<HotelResponseDto>
            {
                new HotelResponseDto { HotelId = Guid.NewGuid(), HotelName = "Test Hotel" }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<CreateHotelsBulkCommand>(), default))
                .ReturnsAsync(createdHotels);

            // Act
            var result = await _controller.CreateHotelsBulk(hotelsDto);

            // Assert
            result.Should().BeOfType<ActionResult<IEnumerable<HotelResponseDto>>>();
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Value.Should().BeEquivalentTo(createdHotels);
            createdResult.ActionName.Should().Be(nameof(_controller.GetAllHotelData));
        }
    }
}