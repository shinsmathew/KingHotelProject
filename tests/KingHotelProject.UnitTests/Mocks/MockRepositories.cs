// KingHotelProject.UnitTests/Mocks/MockRepositories.cs
using KingHotelProject.Core.Entities;
using KingHotelProject.Core.Enums;
using KingHotelProject.Core.Interfaces;
using Moq;

namespace KingHotelProject.UnitTests.Mocks
{
    public static class MockRepositories
    {
        public static Mock<IHotelRepository> GetHotelRepository()
        {
            var hotels = new List<Hotel>
            {
                new Hotel
                {
                    HotelId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                    HotelName = "Grand Hotel",
                    Address = "123 Main St",
                    City = "New York",
                    Zip = "10001",
                    Country = "USA",
                    Email = "info@grandhotel.com",
                    PhoneNumber1 = "+1234567890",
                    CreatedDate = DateTime.UtcNow.AddDays(-10),
                    Dishes = new List<Dish>()
                },
                new Hotel
                {
                    HotelId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                    HotelName = "Luxury Resort",
                    Address = "456 Beach Rd",
                    City = "Miami",
                    Zip = "33139",
                    Country = "USA",
                    Email = "info@luxuryresort.com",
                    PhoneNumber1 = "+1987654321",
                    CreatedDate = DateTime.UtcNow.AddDays(-5),
                    Dishes = new List<Dish>()
                }
            };

            var mockRepo = new Mock<IHotelRepository>();

            mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(hotels);

            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => hotels.FirstOrDefault(h => h.HotelId == id));

            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Hotel>()))
                .ReturnsAsync((Hotel hotel) =>
                {
                    hotels.Add(hotel);
                    return hotel;
                });

            mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Hotel>()))
                .Returns(Task.CompletedTask);

            mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Hotel>()))
                .Returns(Task.CompletedTask)
                .Callback<Hotel>((hotel) => hotels.Remove(hotel));

            return mockRepo;
        }

        public static Mock<IDishRepository> GetDishRepository()
        {
            var hotelId1 = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var hotelId2 = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7");

            var dishes = new List<Dish>
            {
                new Dish
                {
                    DishId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa8"),
                    DishName = "Pasta Carbonara",
                    Price = 15,
                    HotelId = hotelId1,
                    CreatedDate = DateTime.UtcNow.AddDays(-8)
                },
                new Dish
                {
                    DishId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa9"),
                    DishName = "Grilled Salmon",
                    Price = 22,
                    HotelId = hotelId1,
                    CreatedDate = DateTime.UtcNow.AddDays(-7)
                },
                new Dish
                {
                    DishId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afaa"),
                    DishName = "Caesar Salad",
                    Price = 12,
                    HotelId = hotelId2,
                    CreatedDate = DateTime.UtcNow.AddDays(-3)
                }
            };

            var mockRepo = new Mock<IDishRepository>();

            mockRepo.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(dishes);

            mockRepo.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => dishes.FirstOrDefault(d => d.DishId == id));

            mockRepo.Setup(repo => repo.GetByHotelIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid hotelId) => dishes.Where(d => d.HotelId == hotelId).ToList());

            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Dish>()))
                .ReturnsAsync((Dish dish) =>
                {
                    dishes.Add(dish);
                    return dish;
                });

            mockRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Dish>()))
                .Returns(Task.CompletedTask);

            mockRepo.Setup(repo => repo.DeleteAsync(It.IsAny<Dish>()))
                .Returns(Task.CompletedTask)
                .Callback<Dish>((dish) => dishes.Remove(dish));

            return mockRepo;
        }

        public static Mock<IUserRepository> GetUserRepository()
        {
            var users = new List<User>
            {
                new User
                {
                    UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afab"),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@example.com",
                    UserName = "johndoe",
                    PasswordHash = "hashed_password_1", // In a real scenario, this would be properly hashed
                    Role = UserRole.Admin,
                    CreatedDate = DateTime.UtcNow.AddDays(-30)
                },
                new User
                {
                    UserId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afac"),
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@example.com",
                    UserName = "janesmith",
                    PasswordHash = "hashed_password_2", // In a real scenario, this would be properly hashed
                    Role = UserRole.Staff,
                    CreatedDate = DateTime.UtcNow.AddDays(-20)
                }
            };

            var mockRepo = new Mock<IUserRepository>();

            mockRepo.Setup(repo => repo.GetByUserNameAsync(It.IsAny<string>()))
                .ReturnsAsync((string userName) => users.FirstOrDefault(u => u.UserName == userName));

            mockRepo.Setup(repo => repo.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((string email) => users.FirstOrDefault(u => u.Email == email));

            mockRepo.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User user) =>
                {
                    users.Add(user);
                    return user;
                });

            return mockRepo;
        }

        public static Mock<ICacheService> GetCacheService()
        {
            var mockCache = new Mock<ICacheService>();

            mockCache.Setup(cache => cache.GetAsync<It.IsAnyType>(It.IsAny<string>()))
                .ReturnsAsync((string key, Type type) => default);

            mockCache.Setup(cache => cache.SetAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()))
                .Returns(Task.CompletedTask);

            mockCache.Setup(cache => cache.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            return mockCache;
        }
    }
}