using KingHotelProject.API.Middleware;
using KingHotelProject.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace KingHotelProject.UnitTests.API.Middleware
{
    public class ExceptionMiddlewareTests
    {
        private readonly Mock<RequestDelegate> _nextMock;
        private readonly Mock<ILogger<ExceptionMiddleware>> _loggerMock;
        private readonly Mock<IHostEnvironment> _envMock;
        private readonly ExceptionMiddleware _middleware;
        private readonly DefaultHttpContext _context;

        public ExceptionMiddlewareTests()
        {
            _nextMock = new Mock<RequestDelegate>();
            _loggerMock = new Mock<ILogger<ExceptionMiddleware>>();
            _envMock = new Mock<IHostEnvironment>();
            _context = new DefaultHttpContext();
            _context.Response.Body = new MemoryStream();

            _middleware = new ExceptionMiddleware(_nextMock.Object, _loggerMock.Object, _envMock.Object);
        }

        [Fact]
        public async Task InvokeAsync_NoException_ShouldNotModifyResponse()
        {
            // Arrange
            _nextMock.Setup(n => n.Invoke(_context)).Returns(Task.CompletedTask);

            // Act
            await _middleware.InvokeAsync(_context);

            // Assert
            _context.Response.StatusCode.Should().Be(200);
            _context.Response.Body.Length.Should().Be(0);
        }

        [Fact]
        public async Task InvokeAsync_ValidationException_ShouldReturnBadRequestWithDetails()
        {
            // Arrange
            var validationFailures = new List<ValidationFailure>
            {
                new ValidationFailure("Property", "Error message")
            };
            var validationException = new ValidationException(validationFailures);

            _nextMock.Setup(n => n.Invoke(_context)).ThrowsAsync(validationException);
            _envMock.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            await _middleware.InvokeAsync(_context);

            // Assert
            _context.Response.StatusCode.Should().Be(400);
            _context.Response.ContentType.Should().Be("application/json");

            var body = await GetResponseBody();
            body.Should().Contain("Validation Error");
            body.Should().Contain("Property");
            body.Should().Contain("Error message");
        }

        [Fact]
        public async Task InvokeAsync_NotFoundException_ShouldReturnNotFoundWithMessage()
        {
            // Arrange
            var notFoundException = new NotFoundException("Entity", "123");

            _nextMock.Setup(n => n.Invoke(_context)).ThrowsAsync(notFoundException);
            _envMock.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            await _middleware.InvokeAsync(_context);

            // Assert
            _context.Response.StatusCode.Should().Be(404);
            _context.Response.ContentType.Should().Be("application/json");

            var body = await GetResponseBody();
            body.Should().Contain("Not Found");
            body.Should().Contain("Entity \"Entity\" (123) was not found.");
        }

        [Fact]
        public async Task InvokeAsync_GenericException_ShouldReturnInternalServerError()
        {
            // Arrange
            var exception = new Exception("Test exception");

            _nextMock.Setup(n => n.Invoke(_context)).ThrowsAsync(exception);
            _envMock.Setup(e => e.IsDevelopment()).Returns(false);

            // Act
            await _middleware.InvokeAsync(_context);

            // Assert
            _context.Response.StatusCode.Should().Be(500);
            _context.Response.ContentType.Should().Be("application/json");

            var body = await GetResponseBody();
            body.Should().Contain("Internal Server Error");
            body.Should().Contain("An unexpected error occurred");
        }

        private async Task<string> GetResponseBody()
        {
            _context.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_context.Response.Body);
            return await reader.ReadToEndAsync();
        }
    }
}