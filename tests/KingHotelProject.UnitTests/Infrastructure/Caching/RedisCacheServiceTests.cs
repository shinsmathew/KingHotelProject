using KingHotelProject.Infrastructure.Caching;
using Moq;
using StackExchange.Redis;
using Xunit;
using FluentAssertions;
using System.Text.Json;
using System.Threading.Tasks;

namespace KingHotelProject.UnitTests.Infrastructure.Caching
{
    public class RedisCacheServiceTests
    {
        private readonly Mock<IConnectionMultiplexer> _redisConnectionMock;
        private readonly Mock<IDatabase> _databaseMock;
        private readonly RedisCacheService _cacheService;

        public RedisCacheServiceTests()
        {
            _redisConnectionMock = new Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();

            _redisConnectionMock.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                .Returns(_databaseMock.Object);

            _cacheService = new RedisCacheService(_redisConnectionMock.Object);
        }

        [Fact]
        public async Task GetRedisCacheAsync_WithExistingKey_ReturnsDeserializedValue()
        {
            // Arrange
            var key = "test-key";
            var expectedValue = new TestObject { Id = 1, Name = "Test" };
            var serializedValue = JsonSerializer.Serialize(expectedValue);

            _databaseMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync(serializedValue);

            // Act
            var result = await _cacheService.GetRedisCacheAsync<TestObject>(key);

            // Assert
            result.Should().BeEquivalentTo(expectedValue);
        }

        [Fact]
        public async Task GetRedisCacheAsync_WithNonExistingKey_ReturnsDefault()
        {
            // Arrange
            var key = "non-existent-key";

            _databaseMock.Setup(d => d.StringGetAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await _cacheService.GetRedisCacheAsync<TestObject>(key);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SetRedisCacheAsync_WithValue_SetsSerializedValue()
        {
            // Arrange
            var key = "test-key";
            var value = new TestObject { Id = 1, Name = "Test" };
            var serializedValue = JsonSerializer.Serialize(value);

            _databaseMock.Setup(d => d.StringSetAsync(
                key,
                serializedValue,
                It.IsAny<TimeSpan?>(),
                It.IsAny<When>(),
                It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            // Act
            await _cacheService.SetRedisCacheAsync(key, value);

            // Assert
            _databaseMock.Verify(d => d.StringSetAsync(
                key,
                It.Is<string>(s => s.Contains("\"Id\":1") && s.Contains("\"Name\":\"Test\""),
                null,
                When.Always,
                CommandFlags.None),
                Times.Once);
        }

        [Fact]
        public async Task RemoveRedisCacheAsync_WithKey_DeletesKey()
        {
            // Arrange
            var key = "test-key";

            _databaseMock.Setup(d => d.KeyDeleteAsync(key, It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            // Act
            await _cacheService.RemoveRedisCacheAsync(key);

            // Assert
            _databaseMock.Verify(d => d.KeyDeleteAsync(key, CommandFlags.None), Times.Once);
        }

        private class TestObject
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}