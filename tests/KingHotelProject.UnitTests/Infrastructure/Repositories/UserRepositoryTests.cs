using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Data;
using Moq;
using Xunit;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KingHotelProject.Infrastructure.Repositories;

namespace KingHotelProject.UnitTests.Infrastructure.Repositories
{
    public class UserRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            _repository = new UserRepository(_contextMock.Object);
        }

        [Fact]
        public async Task AddUserAsync_ShouldAddUser()
        {
            // Arrange
            var user = new User { UserName = "newuser", Email = "newuser@example.com" };

            _contextMock.Setup(c => c.Users.AddAsync(user, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.AddUserAsync(user);

            // Assert
            _contextMock.Verify(c => c.Users.AddAsync(user, It.IsAny<CancellationToken>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByUserNameAsync_WithExistingUsername_ShouldReturnUser()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid(), UserName = "testuser", Email = "test@example.com" };

            _contextMock.Setup(c => c.Users)
                .ReturnsDbSet(new List<User> { user });

            // Act
            var result = await _repository.GetByUserNameAsync(user.UserName);

            // Assert
            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetByUserNameAsync_WithNonExistingUsername_ShouldReturnNull()
        {
            // Arrange
            var username = "nonexistentuser";

            _contextMock.Setup(c => c.Users)
                .ReturnsDbSet(new List<User>());

            // Act
            var result = await _repository.GetByUserNameAsync(username);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmailAsync_WithExistingEmail_ShouldReturnUser()
        {
            // Arrange
            var user = new User { UserId = Guid.NewGuid(), UserName = "testuser", Email = "test@example.com" };

            _contextMock.Setup(c => c.Users)
                .ReturnsDbSet(new List<User> { user });

            // Act
            var result = await _repository.GetByEmailAsync(user.Email);

            // Assert
            result.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetByEmailAsync_WithNonExistingEmail_ShouldReturnNull()
        {
            // Arrange
            var email = "nonexistent@example.com";

            _contextMock.Setup(c => c.Users)
                .ReturnsDbSet(new List<User>());

            // Act
            var result = await _repository.GetByEmailAsync(email);

            // Assert
            result.Should().BeNull();
        }
    }

    public static class DbSetMockExtensions
    {
        public static DbSet<T> ReturnsDbSet<T>(this Mock<ApplicationDbContext> mockContext, List<T> sourceList) where T : class
        {
            var queryable = sourceList.AsQueryable();

            var dbSet = new Mock<DbSet<T>>();
            dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

            dbSet.Setup(d => d.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Callback<T, CancellationToken>((entity, token) => sourceList.Add(entity))
                .Returns(Task.CompletedTask);

            dbSet.Setup(d => d.Remove(It.IsAny<T>()))
                .Callback<T>(entity => sourceList.Remove(entity));

            dbSet.Setup(d => d.Update(It.IsAny<T>()))
                .Callback<T>(entity => sourceList[sourceList.IndexOf(entity)] = entity);

            mockContext.Setup(c => c.Set<T>()).Returns(dbSet.Object);

            return dbSet.Object;
        }
    }
}