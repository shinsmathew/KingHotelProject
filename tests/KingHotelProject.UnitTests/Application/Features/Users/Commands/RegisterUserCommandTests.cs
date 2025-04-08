using KingHotelProject.Application.DTOs.Users;
using KingHotelProject.Application.Features.Users.Commands;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Enums;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;

namespace KingHotelProject.UnitTests.Application.Features.Users.Commands
{
    public class RegisterUserCommandTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IValidator<UserRegisterDto>> _validatorMock;
        private readonly RegisterUserCommandHandler _handler;

        public RegisterUserCommandTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _validatorMock = new Mock<IValidator<UserRegisterDto>>();
            _handler = new RegisterUserCommandHandler(
                _userRepositoryMock.Object,
                _identityServiceMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldRegisterUserAndReturnAuthResponse()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                UserRegisterDto = new UserRegisterDto
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@example.com",
                    UserName = "testuser",
                    Password = "Password123!",
                    Role = 0
                }
            };

            var hashedPassword = "hashedPassword";
            var token = "test-token";

            _validatorMock.Setup(v => v.ValidateAsync(request.UserRegisterDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(r => r.GetByUserNameAsync(request.UserRegisterDto.UserName))
                .ReturnsAsync((User)null);

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.UserRegisterDto.Email))
                .ReturnsAsync((User)null);

            _identityServiceMock.Setup(s => s.HashPassword(request.UserRegisterDto.Password))
                .Returns(hashedPassword);

            _identityServiceMock.Setup(s => s.GenerateJwtToken(It.IsAny<User>()))
                .Returns(token);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(token);
            result.User.Should().NotBeNull();
            _userRepositoryMock.Verify(r => r.AddUserAsync(It.Is<User>(u =>
                u.UserName == request.UserRegisterDto.UserName &&
                u.PasswordHash == hashedPassword)), Times.Once);
        }

        [Fact]
        public async Task Handle_WithExistingUsername_ShouldThrowBadRequestException()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                UserRegisterDto = new UserRegisterDto
                {
                    UserName = "existinguser",
                    Email = "test@example.com",
                    Password = "Password123!"
                }
            };

            _validatorMock.Setup(v => v.ValidateAsync(request.UserRegisterDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(r => r.GetByUserNameAsync(request.UserRegisterDto.UserName))
                .ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithExistingEmail_ShouldThrowBadRequestException()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                UserRegisterDto = new UserRegisterDto
                {
                    UserName = "testuser",
                    Email = "existing@example.com",
                    Password = "Password123!"
                }
            };

            _validatorMock.Setup(v => v.ValidateAsync(request.UserRegisterDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(r => r.GetByUserNameAsync(request.UserRegisterDto.UserName))
                .ReturnsAsync((User)null);

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.UserRegisterDto.Email))
                .ReturnsAsync(new User());

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidRole_ShouldThrowBadRequestException()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                UserRegisterDto = new UserRegisterDto
                {
                    UserName = "testuser",
                    Email = "test@example.com",
                    Password = "Password123!",
                    Role = 99 // Invalid role
                }
            };

            _validatorMock.Setup(v => v.ValidateAsync(request.UserRegisterDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(r => r.GetByUserNameAsync(request.UserRegisterDto.UserName))
                .ReturnsAsync((User)null);

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.UserRegisterDto.Email))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(() =>
                _handler.Handle(request, CancellationToken.None));
        }
    }
}