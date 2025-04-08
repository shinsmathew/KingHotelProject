using KingHotelProject.Application.DTOs.Dishes;
using KingHotelProject.Application.Features.Dishes.Validators;
using FluentValidation.TestHelper;
using Xunit;
using System.Collections.Generic;

namespace KingHotelProject.UnitTests.Application.Features.Dishes.Validators
{
    public class DishValidatorTests
    {
        private readonly DishesBulkCreateValidator _bulkCreateValidator;
        private readonly DishUpdateValidator _updateValidator;

        public DishValidatorTests()
        {
            _bulkCreateValidator = new DishesBulkCreateValidator();
            _updateValidator = new DishUpdateValidator();
        }

        [Fact]
        public void DishesBulkCreateValidator_EmptyDishes_ShouldHaveError()
        {
            // Arrange
            var model = new DishesBulkCreateDto { Dishes = new List<DishCreateDto>() };

            // Act
            var result = _bulkCreateValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Dishes)
                .WithErrorMessage("At least one dish is required");
        }

        [Fact]
        public void DishesBulkCreateValidator_TooManyDishes_ShouldHaveError()
        {
            // Arrange
            var dishes = new List<DishCreateDto>();
            for (int i = 0; i < 101; i++)
            {
                dishes.Add(new DishCreateDto());
            }

            var model = new DishesBulkCreateDto { Dishes = dishes };

            // Act
            var result = _bulkCreateValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Dishes)
                .WithErrorMessage("Cannot create more than 100 dishes at once");
        }

        [Fact]
        public void DishesBulkCreateValidator_ValidDish_ShouldNotHaveError()
        {
            // Arrange
            var model = new DishesBulkCreateDto
            {
                Dishes = new List<DishCreateDto>
                {
                    new DishCreateDto
                    {
                        DishName = "Valid Dish",
                        Price = 10.99m,
                        HotelId = System.Guid.NewGuid()
                    }
                }
            };

            // Act
            var result = _bulkCreateValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void DishUpdateValidator_EmptyDishName_ShouldHaveError()
        {
            // Arrange
            var model = new DishUpdateDto { DishName = "" };

            // Act
            var result = _updateValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.DishName)
                .WithErrorMessage("Dish name is required");
        }

        [Fact]
        public void DishUpdateValidator_ValidData_ShouldNotHaveError()
        {
            // Arrange
            var model = new DishUpdateDto
            {
                DishName = "Valid Dish",
                Price = 10.99m
            };

            // Act
            var result = _updateValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}