using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.Features.Hotels.Commands;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using AutoMapper;
using FluentValidation;
using FluentValidation.Results;

namespace KingHotelProject.UnitTests.Application.Features.Hotels.Commands
{
    public class CreateHotelsBulkCommandTests
    {
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<IValidator<HotelsBulkCreateDto>> _validatorMock;
        private readonly CreateHotelsBulkCommandHandler _handler;

        public CreateHotelsBulkCommandTests()
        {
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _mapperMock = new Mock<IMapper>();
            _cacheServiceMock = new Mock<ICacheService>();
            _validatorMock = new Mock<IValidator<HotelsBulkCreateDto>>();
            _handler = new CreateHotelsBulkCommandHandler(
                _hotelRepositoryMock.Object,
                _mapperMock.Object,
                _cacheServiceMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldCreateHotelsAndClearCache()
        {
            // Arrange
            var request = new CreateHotelsBulkCommand
            {
                HotelsBulkCreateDto = new HotelsBulkCreateDto
                {
                    Hotels = new List<HotelCreateDto>
                    {
                        new HotelCreateDto { HotelName = "Test Hotel" }
                    }
                }
            };

            var hotel = new Hotel { HotelId = Guid.NewGuid(), HotelName = "Test Hotel" };
            var hotelResponse = new HotelResponseDto { HotelId = hotel.HotelId, HotelName = hotel.HotelName };

            _validatorMock.Setup(v => v.ValidateAsync(request.HotelsBulkCreateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mapperMock.Setup(m => m.Map<List<Hotel>>(request.HotelsBulkCreateDto.Hotels))
                .Returns(new List<Hotel> { hotel });

            _hotelRepositoryMock.Setup(r => r.AddHotelAsync(It.IsAny<Hotel>()))
                .ReturnsAsync(hotel);

            _mapperMock.Setup(m => m.Map<IEnumerable<HotelResponseDto>>(It.IsAny<IEnumerable<Hotel>>()))
                .Returns(new List<HotelResponseDto> { hotelResponse });

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().HaveCount(1);
            result.First().Should().BeEquivalentTo(hotelResponse);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync("AllHotels"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var request = new CreateHotelsBulkCommand
            {
                HotelsBulkCreateDto = new HotelsBulkCreateDto()
            };

            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Name", "Error") };

            _validatorMock.Setup(v => v.ValidateAsync(request.HotelsBulkCreateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }
    }
}