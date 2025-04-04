@echo off
echo Creating solution and folders...

dotnet new sln -n KingHotelProject
mkdir src
cd src

echo Creating main projects...
dotnet new webapi -n KingHotelProject.API
dotnet new classlib -n KingHotelProject.Core
dotnet new classlib -n KingHotelProject.Application
dotnet new classlib -n KingHotelProject.Infrastructure

cd ..
mkdir tests
cd tests

echo Creating test projects...
dotnet new xunit -n KingHotelProject.UnitTests
dotnet new xunit -n KingHotelProject.IntegrationTests

cd ..

echo Adding projects to solution...
dotnet sln add src/KingHotelProject.API/KingHotelProject.API.csproj
dotnet sln add src/KingHotelProject.Core/KingHotelProject.Core.csproj
dotnet sln add src/KingHotelProject.Application/KingHotelProject.Application.csproj
dotnet sln add src/KingHotelProject.Infrastructure/KingHotelProject.Infrastructure.csproj
dotnet sln add tests/KingHotelProject.UnitTests/KingHotelProject.UnitTests.csproj
dotnet sln add tests/KingHotelProject.IntegrationTests/KingHotelProject.IntegrationTests.csproj

echo Adding project references...
dotnet add src/KingHotelProject.API/KingHotelProject.API.csproj reference src/KingHotelProject.Application/KingHotelProject.Application.csproj

dotnet add src/KingHotelProject.Application/KingHotelProject.Application.csproj reference src/KingHotelProject.Core/KingHotelProject.Core.csproj
dotnet add src/KingHotelProject.Application/KingHotelProject.Application.csproj reference src/KingHotelProject.Infrastructure/KingHotelProject.Infrastructure.csproj

dotnet add src/KingHotelProject.Infrastructure/KingHotelProject.Infrastructure.csproj reference src/KingHotelProject.Core/KingHotelProject.Core.csproj

dotnet add tests/KingHotelProject.UnitTests/KingHotelProject.UnitTests.csproj reference src/KingHotelProject.API/KingHotelProject.API.csproj
dotnet add tests/KingHotelProject.UnitTests/KingHotelProject.UnitTests.csproj reference src/KingHotelProject.Application/KingHotelProject.Application.csproj
dotnet add tests/KingHotelProject.IntegrationTests/KingHotelProject.IntegrationTests.csproj reference src/KingHotelProject.API/KingHotelProject.API.csproj

echo Installing NuGet packages...

REM --- API Packages ---
cd src/KingHotelProject.API
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add package StackExchange.Redis
dotnet add package Serilog.AspNetCore
dotnet add package Swashbuckle.AspNetCore

REM --- Application Packages ---
cd ../KingHotelProject.Application
dotnet add package MediatR
dotnet add package AutoMapper
dotnet add package FluentValidation

REM --- Infrastructure Packages ---
cd ../KingHotelProject.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore

REM --- Test Projects ---
cd ../../tests/KingHotelProject.UnitTests
dotnet add package Moq
dotnet add package FluentAssertions
dotnet add package xunit.runner.visualstudio

cd ../KingHotelProject.IntegrationTests
dotnet add package Microsoft.AspNetCore.Mvc.Testing
dotnet add package FluentAssertions

echo.
echo ✅ Setup Complete!
pause
