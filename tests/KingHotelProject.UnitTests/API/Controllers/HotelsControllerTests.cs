using KingHotelProject.API.Controllers;
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.Features.Hotels.Commands;
using KingHotelProject.Application.Features.Hotels.Queries;
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
        public async Task GetAllHotelData_ReturnsOkResultWIthHotels()
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


        [Fact]
        public async Task UpdateHotel_WithValidData_ReturnsNoContent()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var hotelUpdateDto = new HotelUpdateDto
            {
                HotelName = "Updated Hotel",
                Address = "456 Updated St",
                City = "Updated City",
                Zip = "54321",
                Country = "Updated Country",
                Email = "updated@example.com",
                PhoneNumber1 = "+9876543210"
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateHotelCommand>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.UpdateHotel(hotelId, hotelUpdateDto);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(m => m.Send(It.Is<UpdateHotelCommand>(c =>
                c.Id == hotelId && c.HotelUpdateDto == hotelUpdateDto), default), Times.Once);
        }

        

        [Fact]
        public async Task DeleteHotel_WithValidId_ReturnsNoContent()
        {
            // Arrange
            var hotelId = Guid.NewGuid();

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteHotelCommand>(), default))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.DeleteHotel(hotelId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mediatorMock.Verify(m => m.Send(It.Is<DeleteHotelCommand>(c =>
                c.Id == hotelId), default), Times.Once);
        }

        

    }
}