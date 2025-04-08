using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.Features.Hotels.Commands;
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

namespace KingHotelProject.UnitTests.Application.Features.Hotels.Commands
{
    public class UpdateHotelCommandTests
    {
        private readonly Mock<IHotelRepository> _hotelRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IValidator<HotelUpdateDto>> _validatorMock;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly UpdateHotelCommandHandler _handler;

        public UpdateHotelCommandTests()
        {
            _hotelRepositoryMock = new Mock<IHotelRepository>();
            _mapperMock = new Mock<IMapper>();
            _validatorMock = new Mock<IValidator<HotelUpdateDto>>();
            _cacheServiceMock = new Mock<ICacheService>();
            _handler = new UpdateHotelCommandHandler(
                _hotelRepositoryMock.Object,
                _mapperMock.Object,
                _validatorMock.Object,
                _cacheServiceMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldUpdateAndClearCache()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var hotel = new Hotel { HotelId = hotelId };
            var updateDto = new HotelUpdateDto { HotelName = "Updated Hotel" };

            _validatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _hotelRepositoryMock.Setup(r => r.GetHotelByIdAsync(hotelId))
                .ReturnsAsync(hotel);

            // Act
            await _handler.Handle(new UpdateHotelCommand { Id = hotelId, HotelUpdateDto = updateDto }, CancellationToken.None);

            // Assert
            _mapperMock.Verify(m => m.Map(updateDto, hotel), Times.Once);
            _hotelRepositoryMock.Verify(r => r.UpdateHotelAsync(hotel), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync("AllHotels"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"Hotel_{hotelId}"), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveRedisCacheAsync($"DishesByHotel_{hotelId}"), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInvalidData_ShouldThrowValidationException()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var updateDto = new HotelUpdateDto();
            var validationFailures = new List<ValidationFailure> { new ValidationFailure("Name", "Error") };

            _validatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult(validationFailures));

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                _handler.Handle(new UpdateHotelCommand { Id = hotelId, HotelUpdateDto = updateDto }, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithNonExistingHotel_ShouldThrowNotFoundException()
        {
            // Arrange
            var hotelId = Guid.NewGuid();
            var updateDto = new HotelUpdateDto { HotelName = "Updated Hotel" };

            _validatorMock.Setup(v => v.ValidateAsync(updateDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _hotelRepositoryMock.Setup(r => r.GetHotelByIdAsync(hotelId))
                .ReturnsAsync((Hotel)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                _handler.Handle(new UpdateHotelCommand { Id = hotelId, HotelUpdateDto = updateDto }, CancellationToken.None));
        }
    }
}