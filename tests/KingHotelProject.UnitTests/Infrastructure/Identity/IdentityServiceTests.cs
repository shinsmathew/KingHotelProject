using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Enums;
using KingHotelProject.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using FluentAssertions;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KingHotelProject.UnitTests.Infrastructure.Identity
{
    public class IdentityServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly IdentityService _identityService;
        private const string TestKey = "kjidhIUOldbaijlhfqwjdLHDJHQWKJDKLdqwbdjblBDQBFGDSzxcvbnmkjhertgbnIUGWIDGLdhefbsabdfhlaflAYWGerfgbnoiuhgfdxDCDAEFZFF";

        public IdentityServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();

            _configurationMock.Setup(c => c["Jwt:Key"]).Returns(TestKey);
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _configurationMock.Setup(c => c["Jwt:ExpiryMinutes"]).Returns("30");

            _identityService = new IdentityService(_configurationMock.Object);
        }

        [Fact]
        public void GenerateJwtToken_WithValidUser_ReturnsValidToken()
        {
            // Arrange
            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = "testuser",
                Email = "test@example.com",
                Role = UserRole.Admin
            };

            // Act
            var token = _identityService.GenerateJwtToken(user);

            // Assert
            token.Should().NotBeNullOrEmpty();

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            jwtToken.Issuer.Should().Be("TestIssuer");
            jwtToken.Audiences.Should().Contain("TestAudience");
            jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "Admin");
            jwtToken.Claims.Should().Contain(c => c.Type == "userId" && c.Value == user.UserId.ToString());
            jwtToken.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void HashPassword_WithValidPassword_ReturnsHash()
        {
            // Arrange
            var password = "TestPassword123!";

            // Act
            var hash = _identityService.HashPassword(password);

            // Assert
            hash.Should().NotBeNullOrEmpty();
            hash.Should().NotBe(password);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "TestPassword123!";
            var hash = _identityService.HashPassword(password);

            // Act
            var result = _identityService.VerifyPassword(hash, password);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "TestPassword123!";
            var wrongPassword = "WrongPassword";
            var hash = _identityService.HashPassword(password);

            // Act
            var result = _identityService.VerifyPassword(hash, wrongPassword);

            // Assert
            result.Should().BeFalse();
        }
    }
}