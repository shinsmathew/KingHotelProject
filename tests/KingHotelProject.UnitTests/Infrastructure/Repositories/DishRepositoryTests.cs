using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Interfaces;
using KingHotelProject.Infrastructure.Data;
using Moq;
using Xunit;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using KingHotelProject.Infrastructure.Repositories;

namespace KingHotelProject.UnitTests.Infrastructure.Repositories
{
    public class DishRepositoryTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly DishRepository _repository;

        public DishRepositoryTests()
        {
            _contextMock = new Mock<ApplicationDbContext>(new DbContextOptionsBuilder<ApplicationDbContext>().Options);
            _repository = new DishRepository(_contextMock.Object);
        }

        [Fact]
        public async Task GetAllDishAsync_ShouldReturnAllDishes()
        {
            // Arrange
            var dishes = new List<Dish>
            {
                new Dish { DishId = Guid.NewGuid(), DishName = "Test Dish 1" },
                new Dish { DishId = Guid.NewGuid(), DishName = "Test Dish 2" }
            };

            _contextMock.Setup(c => c.Dishes)
                .ReturnsDbSet(dishes);

            // Act
            var result = await _repository.GetAllDishAsync();

            // Assert
            result.Should().BeEquivalentTo(dishes);
        }

        [Fact]
        public async Task GetDishByIdAsync_WithExistingId_ShouldReturnDish()
        {
            // Arrange
            var dish = new Dish { DishId = Guid.NewGuid(), DishName = "Test Dish" };

            _contextMock.Setup(c => c.Dishes)
                .ReturnsDbSet(new List<Dish> { dish });

            // Act
            var result = await _repository.GetDishByIdAsync(dish.DishId);

            // Assert
            result.Should().BeEquivalentTo(dish);
        }

        [Fact]
        public async Task GetDishByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Arrange
            var dishId = Guid.NewGuid();

            _contextMock.Setup(c => c.Dishes)
                .ReturnsDbSet(new List<Dish>());

            // Act
            var result = await _repository.GetDishByIdAsync(dishId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task AddDishAsync_ShouldAddDish()
        {
            // Arrange
            var dish = new Dish { DishName = "New Dish" };

            _contextMock.Setup(c => c.Dishes.AddAsync(dish, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.AddDishAsync(dish);

            // Assert
            _contextMock.Verify(c => c.Dishes.AddAsync(dish, It.IsAny<CancellationToken>()), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task UpdateDishAsync_ShouldUpdateDish()
        {
            // Arrange
            var dish = new Dish { DishId = Guid.NewGuid(), DishName = "Updated Dish" };

            _contextMock.Setup(c => c.Dishes.Update(dish))
                .Returns(_contextMock.Object.Entry(dish));

            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.UpdateDishAsync(dish);

            // Assert
            _contextMock.Verify(c => c.Dishes.Update(dish), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteDishAsync_ShouldDeleteDish()
        {
            // Arrange
            var dish = new Dish { DishId = Guid.NewGuid(), DishName = "Test Dish" };

            _contextMock.Setup(c => c.Dishes.Remove(dish))
                .Returns(_contextMock.Object.Entry(dish));

            _contextMock.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _repository.DeleteDishAsync(dish);

            // Assert
            _contextMock.Verify(c => c.Dishes.Remove(dish), Times.Once);
            _contextMock.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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