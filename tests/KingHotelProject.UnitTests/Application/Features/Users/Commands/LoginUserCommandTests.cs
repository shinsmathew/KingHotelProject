using KingHotelProject.Application.DTOs.Users;
using KingHotelProject.Application.Features.Users.Commands;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using KingHotelProject.Core.Enums;
using FluentValidation;
using FluentValidation.Results;

namespace KingHotelProject.UnitTests.Application.Features.Users.Commands
{
    public class LoginUserCommandTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IIdentityService> _identityServiceMock;
        private readonly Mock<IValidator<UserLoginDto>> _validatorMock;
        private readonly LoginUserCommandHandler _handler;

        public LoginUserCommandTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _identityServiceMock = new Mock<IIdentityService>();
            _validatorMock = new Mock<IValidator<UserLoginDto>>();

            _handler = new LoginUserCommandHandler(
                _userRepositoryMock.Object,
                _identityServiceMock.Object,
                _validatorMock.Object);
        }

        [Fact]
        public async Task Handle_WithValidCredentials_ReturnsAuthResponse()
        {
            // Arrange
            var request = new LoginUserCommand
            {
                UserLoginDto = new UserLoginDto
                {
                    UserName = "testuser",
                    Password = "Password123!"
                }
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "testuser",
                PasswordHash = "hashedpassword",
                Role = UserRole.Admin
            };

            var expectedToken = "test-token";

            _validatorMock.Setup(v => v.ValidateAsync(request.UserLoginDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(r => r.GetByUserNameAsync(request.UserLoginDto.UserName))
                .ReturnsAsync(user);

            _identityServiceMock.Setup(s => s.VerifyPassword(user.PasswordHash, request.UserLoginDto.Password))
                .Returns(true);

            _identityServiceMock.Setup(s => s.GenerateJwtToken(user))
                .Returns(expectedToken);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(expectedToken);
            result.User.Should().NotBeNull();
            result.User.UserId.Should().Be(user.UserId);
            result.User.UserName.Should().Be(user.UserName);
        }

        [Fact]
        public async Task Handle_WithInvalidUsername_ThrowsUnauthorizedException()
        {
            // Arrange
            var request = new LoginUserCommand
            {
                UserLoginDto = new UserLoginDto
                {
                    UserName = "nonexistent",
                    Password = "Password123!"
                }
            };

            _validatorMock.Setup(v => v.ValidateAsync(request.UserLoginDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(r => r.GetByUserNameAsync(request.UserLoginDto.UserName))
                .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<Core.Exceptions.UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_WithInvalidPassword_ThrowsUnauthorizedException()
        {
            // Arrange
            var request = new LoginUserCommand
            {
                UserLoginDto = new UserLoginDto
                {
                    UserName = "testuser",
                    Password = "WrongPassword"
                }
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "testuser",
                PasswordHash = "hashedpassword",
                Role = UserRole.Admin
            };

            _validatorMock.Setup(v => v.ValidateAsync(request.UserLoginDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(r => r.GetByUserNameAsync(request.UserLoginDto.UserName))
                .ReturnsAsync(user);

            _identityServiceMock.Setup(s => s.VerifyPassword(user.PasswordHash, request.UserLoginDto.Password))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<Core.Exceptions.UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
        }
    }
}