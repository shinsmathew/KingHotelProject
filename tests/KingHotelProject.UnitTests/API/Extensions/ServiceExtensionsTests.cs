using KingHotelProject.API.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using KingHotelProject.Infrastructure.Data;
using StackExchange.Redis;
using FluentValidation;
using KingHotelProject.Application.Features.Hotels.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MediatR;
using KingHotelProject.Application.Features.Hotels.Queries;
using AutoMapper;
using KingHotelProject.Application.Mappings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System;
using KingHotelProject.Application.DTOs.Hotels;
using KingHotelProject.Core.Interfaces;

namespace KingHotelProject.UnitTests.API.Extensions
{
    public class ServiceExtensionsTests
    {
        private readonly ServiceCollection _services;
        private readonly IConfiguration _configuration;

        public ServiceExtensionsTests()
        {
            _services = new ServiceCollection();
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"ConnectionStrings:DefaultConnection", "TestConnectionString"},
                    {"ConnectionStrings:Redis", "localhost:6379"},
                    {"Jwt:Key", "TestKey"},
                    {"Jwt:Issuer", "TestIssuer"},
                    {"Jwt:Audience", "TestAudience"},
                    {"Jwt:ExpiryMinutes", "30"}
                })
                .Build();
        }

        [Fact]
        public void ConfigureDbContext_ShouldRegisterDbContext()
        {
            // Act
            _services.ConfigureDbContext(_configuration);

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var dbContext = serviceProvider.GetService<ApplicationDbContext>();
            dbContext.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureRedis_ShouldRegisterRedisServices()
        {
            // Act
            _services.ConfigureRedis(_configuration);

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var redis = serviceProvider.GetService<IConnectionMultiplexer>();
            var cacheService = serviceProvider.GetService<ICacheService>();

            redis.Should().NotBeNull();
            cacheService.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureRepositories_ShouldRegisterRepositories()
        {
            // Act
            _services.ConfigureRepositories();

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var hotelRepo = serviceProvider.GetService<IHotelRepository>();
            var dishRepo = serviceProvider.GetService<IDishRepository>();
            var userRepo = serviceProvider.GetService<IUserRepository>();

            hotelRepo.Should().NotBeNull();
            dishRepo.Should().NotBeNull();
            userRepo.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureJwt_ShouldRegisterAuthentication()
        {
            // Act
            _services.ConfigureJwt(_configuration);

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var authOptions = serviceProvider.GetService<JwtBearerOptions>();

            authOptions.Should().NotBeNull();
            authOptions.TokenValidationParameters.ValidateIssuer.Should().BeTrue();
            authOptions.TokenValidationParameters.ValidIssuer.Should().Be("TestIssuer");
        }

        [Fact]
        public void ConfigureMediatR_ShouldRegisterMediatR()
        {
            // Act
            _services.ConfigureMediatR();

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var mediator = serviceProvider.GetService<IMediator>();

            mediator.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureAutoMapper_ShouldRegisterAutoMapper()
        {
            // Act
            _services.ConfigureAutoMapper();

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var mapper = serviceProvider.GetService<IMapper>();

            mapper.Should().NotBeNull();
        }

        [Fact]
        public void ConfigureFluentValidation_ShouldRegisterValidators()
        {
            // Act
            _services.ConfigureFluentValidation();

            // Assert
            var serviceProvider = _services.BuildServiceProvider();
            var validator = serviceProvider.GetService<IValidator<HotelsBulkCreateDto>>();

            validator.Should().NotBeNull();
        }
    }
}