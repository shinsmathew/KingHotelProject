using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Application.Features.Hotels.Validators;
using FluentValidation.TestHelper;
using Xunit;
using System.Collections.Generic;

namespace KingHotelProject.UnitTests.Application.Features.Hotels.Validators
{
    public class HotelValidatorTests
    {
        private readonly HotelsBulkCreateValidator _bulkCreateValidator;
        private readonly HotelUpdateValidator _updateValidator;

        public HotelValidatorTests()
        {
            _bulkCreateValidator = new HotelsBulkCreateValidator();
            _updateValidator = new HotelUpdateValidator();
        }

        [Fact]
        public void HotelsBulkCreateValidator_EmptyHotels_ShouldHaveError()
        {
            // Arrange
            var model = new HotelsBulkCreateDto { Hotels = new List<HotelCreateDto>() };

            // Act
            var result = _bulkCreateValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Hotels)
                .WithErrorMessage("At least one hotel is required");
        }

        [Fact]
        public void HotelsBulkCreateValidator_TooManyHotels_ShouldHaveError()
        {
            // Arrange
            var hotels = new List<HotelCreateDto>();
            for (int i = 0; i < 51; i++)
            {
                hotels.Add(new HotelCreateDto());
            }

            var model = new HotelsBulkCreateDto { Hotels = hotels };

            // Act
            var result = _bulkCreateValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Hotels)
                .WithErrorMessage("Cannot create more than 50 hotels at once");
        }

        [Fact]
        public void HotelsBulkCreateValidator_ValidHotel_ShouldNotHaveError()
        {
            // Arrange
            var model = new HotelsBulkCreateDto
            {
                Hotels = new List<HotelCreateDto>
                {
                    new HotelCreateDto
                    {
                        HotelName = "Valid Hotel",
                        Address = "123 Test St",
                        City = "Test City",
                        Zip = "12345",
                        Country = "Test Country",
                        Email = "test@example.com",
                        PhoneNumber1 = "+1234567890"
                    }
                }
            };

            // Act
            var result = _bulkCreateValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void HotelUpdateValidator_EmptyHotelName_ShouldHaveError()
        {
            // Arrange
            var model = new HotelUpdateDto { HotelName = "" };

            // Act
            var result = _updateValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.HotelName)
                .WithErrorMessage("Hotel name is required");
        }

        [Fact]
        public void HotelUpdateValidator_ValidData_ShouldNotHaveError()
        {
            // Arrange
            var model = new HotelUpdateDto
            {
                HotelName = "Valid Hotel",
                Address = "123 Test St",
                City = "Test City",
                Zip = "12345",
                Country = "Test Country",
                Email = "test@example.com",
                PhoneNumber1 = "+1234567890"
            };

            // Act
            var result = _updateValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}