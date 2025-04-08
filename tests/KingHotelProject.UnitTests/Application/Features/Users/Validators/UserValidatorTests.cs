using KingHotelProject.Application.DTOs.Users;
using KingHotelProject.Application.Features.Users.Validators;
using FluentValidation.TestHelper;
using Xunit;

namespace KingHotelProject.UnitTests.Application.Features.Users.Validators
{
    public class UserValidatorTests
    {
        private readonly RegisterUserValidator _registerValidator;
        private readonly UserLoginValidator _loginValidator;

        public UserValidatorTests()
        {
            _registerValidator = new RegisterUserValidator();
            _loginValidator = new UserLoginValidator();
        }

        [Fact]
        public void RegisterUserValidator_WeakPassword_ShouldHaveError()
        {
            // Arrange
            var model = new UserRegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                UserName = "testuser",
                Password = "weak",
                Role = 0
            };

            // Act
            var result = _registerValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Password)
                .WithErrorMessage("Password must be at least 8 characters");
        }

        [Fact]
        public void RegisterUserValidator_ValidData_ShouldNotHaveError()
        {
            // Arrange
            var model = new UserRegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                UserName = "testuser",
                Password = "StrongPassword123!",
                Role = 0
            };

            // Act
            var result = _registerValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void UserLoginValidator_EmptyUsername_ShouldHaveError()
        {
            // Arrange
            var model = new UserLoginDto { UserName = "", Password = "password" };

            // Act
            var result = _loginValidator.TestValidate(model);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.UserName)
                .WithErrorMessage("Username is required");
        }

        [Fact]
        public void UserLoginValidator_ValidData_ShouldNotHaveError()
        {
            // Arrange
            var model = new UserLoginDto { UserName = "testuser", Password = "password" };

            // Act
            var result = _loginValidator.TestValidate(model);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}