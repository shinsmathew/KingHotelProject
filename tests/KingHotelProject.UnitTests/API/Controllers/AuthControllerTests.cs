using KingHotelProject.API.Controllers;
using KingHotelProject.Application.DTOs.Users;
using KingHotelProject.Application.Features.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;

namespace KingHotelProject.UnitTests.API.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new AuthController(_mediatorMock.Object);
        }

        [Fact]
        public async Task Register_WithValidData_ReturnsOkResult()
        {
            // Arrange
            var userRegisterDto = new UserRegisterDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                UserName = "testuser",
                Password = "Password123!",
                Role = 0
            };

            var authResponse = new AuthResponseDto
            {
                Token = "test-token",
                User = new UserResponseDto()
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<RegisterUserCommand>(), default))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Register(userRegisterDto);

            // Assert
            result.Should().BeOfType<ActionResult<AuthResponseDto>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(authResponse);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var userLoginDto = new UserLoginDto
            {
                UserName = "testuser",
                Password = "Password123!"
            };

            var authResponse = new AuthResponseDto
            {
                Token = "test-token",
                User = new UserResponseDto()
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<LoginUserCommand>(), default))
                .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(userLoginDto);

            // Assert
            result.Should().BeOfType<ActionResult<AuthResponseDto>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(authResponse);
        }
    }
}