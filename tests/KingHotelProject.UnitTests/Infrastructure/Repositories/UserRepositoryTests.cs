using KingHotelProject.Core.Entities;
using KingHotelProject.Infrastructure.Data;
using KingHotelProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using System;

namespace KingHotelProject.UnitTests.Infrastructure.Repositories
{
    public class UserRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new UserRepository(_context);

            // Seed test data
            _context.Users.Add(new User
            {
                UserId = Guid.NewGuid(),
                UserName = "existinguser",
                Email = "existing@example.com"
            });
            _context.SaveChanges();
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUser()
        {
            // Arrange
            var user = new User
            {
                UserName = "newuser",
                Email = "new@example.com"
            };

            // Act
            var result = await _repository.AddUserAsync(user);

            // Assert
            result.Should().NotBeNull();
            result.UserId.Should().NotBe(Guid.Empty);
            (await _repository.GetByUserNameAsync("newuser")).Should().NotBeNull();
        }

        [Fact]
        public async Task GetByUserNameAsync_WithExistingUser_ShouldReturnUser()
        {
            // Act
            var result = await _repository.GetByUserNameAsync("existinguser");

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be("existinguser");
        }

        [Fact]
        public async Task GetByUserNameAsync_WithNonExistingUser_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByUserNameAsync("nonexistent");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
        {
            // Act
            var result = await _repository.GetByEmailAsync("existing@example.com");

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be("existing@example.com");
        }

        [Fact]
        public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByEmailAsync("nonexistent@example.com");

            // Assert
            result.Should().BeNull();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}