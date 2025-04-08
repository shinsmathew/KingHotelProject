using KingHotelProject.Application.Features.Users.Commands;
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Exceptions;
using KingHotelProject.Core.Interfaces;
using Moq;
using Xunit;
using FluentAssertions;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using KingHotelProject.Application.DTOs.Users;
using KingHotelProject.Core.Enums;

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
        public async Task Handle_WithValidData_ReturnsAuthResponse()
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
                    Password = "StrongPassword123!",
                    Role = 0
                }
            };

            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = request.UserRegisterDto.UserName,
                Email = request.UserRegisterDto.Email,
                Role = (UserRole)request.UserRegisterDto.Role
            };

            var authResponse = new AuthResponseDto
            {
                Token = "test-token",
                User = new UserResponseDto
                {
                    UserId = user.UserId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = user.Role,
                    CreatedDate = user.CreatedDate
                }
            };

            _validatorMock.Setup(v => v.ValidateAsync(request.UserRegisterDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _userRepositoryMock.Setup(r => r.GetByUserNameAsync(request.UserRegisterDto.UserName))
                .ReturnsAsync((User)null);

            _userRepositoryMock.Setup(r => r.GetByEmailAsync(request.UserRegisterDto.Email))
                .ReturnsAsync((User)null);

            _userRepositoryMock.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            _identityServiceMock.Setup(s => s.GenerateJwtToken(user))
                .Returns(authResponse.Token);

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task Handle_WithExistingUsername_ThrowsBadRequestException()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                UserRegisterDto = new UserRegisterDto
                {
                    UserName = "existinguser",
                    Email = "test@example.com",
                    Password = "StrongPassword123!",
                    Role = 0
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
        public async Task Handle_WithExistingEmail_ThrowsBadRequestException()
        {
            // Arrange
            var request = new RegisterUserCommand
            {
                UserRegisterDto = new UserRegisterDto
                {
                    UserName = "testuser",
                    Email = "existing@example.com",
                    Password = "StrongPassword123!",
                    Role = 0
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
    }
}