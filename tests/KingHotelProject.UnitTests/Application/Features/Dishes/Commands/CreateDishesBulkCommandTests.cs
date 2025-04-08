using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.Features.Dishes.Commands;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using FluentAssertions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;

namespace KingHotelProject.UnitTests.Application.Features.Dishes.Commands
{
    public class CreateDishesBulkCommandTests
    {
        private readonly Mock<IDishRepository> _dishRepositoryMock;
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IValidator<DishesBulkCreateDto>> _validatorMock;
        private readonly CreateDishesBulkCommandHandler _handler;

        public CreateDishesBulkCommandTests()
        {
            _dishRepositoryMock = new Mock<IDishRepository>();
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
            _validatorMock = new Mock<IValidator<DishesBulkCreateDto>>();

            _handler = new CreateDishesBulkCommandHandler(
                _dishRepositoryMock.Object,
                _hotelRepositoryMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsDishes()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var request = new CreateDishesBulkCommand
            {
                DishesBulkCreateDto = new DishesBulkCreateDto
                {
                    Dishes = new List<DishCreateDto>
                    {
                        new DishCreateDto { DishName = "Test Dish", Price = 10.99m, HotelId = hotelId }
                    }
                }
            };

            var hotel = new Hotel { HotelId = hotelId };
            var dish = new Dish { DishId = Guid.NewGuid(), DishName = "Test Dish", HotelId = hotelId };
            var dishResponse = new DishResponseDto { DishId = dish.DishId, DishName = dish.DishName };

            _validatorMock.Setup(v => v.ValidateAsync(request.DishesBulkCreateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _hotelRepositoryMock.Setup(r => r.GetHotelByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(new List<Hotel> { hotel });

            _mapperMock.Setup(m => m.Map<List<Dish>>(request.DishesBulkCreateDto.Dishes))
                .Returns(new List<Dish> { dish });

            _dishRepositoryMock.Setup(r => r.AddDishAsync(It.IsAny<Dish>()))
                .ReturnsAsync(dish);

            _mapperMock.Setup(m => m.Map<IEnumerable<DishResponseDto>>(It.IsAny<IEnumerable<Dish>>()))
                .Returns(new List<DishResponseDto> { dishResponse });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.First().Should().BeEquivalentTo(dishResponse);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync("AllDishes"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"DishesByHotel_{hotelId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidHotel_ThrowsNotFoundException()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var request = new CreateDishesBulkCommand
            {
                DishesBulkCreateDto = new DishesBulkCreateDto
                {
                    Dishes = new List<DishCreateDto>
                    {
                        new DishCreateDto { DishName = "Test Dish", Price = 10.99m, HotelId = hotelId }
                    }
                }
            };

            _validatorMock.Setup(v => v.ValidateAsync(request.DishesBulkCreateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _hotelRepositoryMock.Setup(r => r.GetHotelByIdsAsync(It.IsAny<IEnumerable<Guid>>()))
                .ReturnsAsync(new List<Hotel>());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _handler.Handle(request, CancellationToken.None));
        }
    }
}