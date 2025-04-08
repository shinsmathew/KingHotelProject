King Hotel Project - REST API

A comprehensive hotel management system API built with ASP.NET Core 8, featuring authentication, authorization, and Redis caching.

Features
User Authentication: JWT-based authentication with role-based authorization (Admin/Staff)

Hotel Management: CRUD operations for hotels with bulk creation support

Dish Management: CRUD operations for dishes with bulk creation support

Redis Caching: Performance optimization with Redis caching layer

Validation: Comprehensive request validation with FluentValidation

Exception Handling: Custom exception middleware with detailed error responses

AutoMapper: Clean DTO to entity mapping

MediatR: CQRS pattern implementation

Swagger: API documentation with JWT support

Architecture:

KingHotelProject/
├── src/
│   ├── KingHotelProject.API/            # Web API layer (Controllers, Middleware)
│   ├── KingHotelProject.Core/           # Domain layer (Entities, Interfaces, Exceptions)
│   ├── KingHotelProject.Application/    # Application layer (DTOs, Commands, Queries, Validators)
│   ├── KingHotelProject.Infrastructure/ # Infrastructure layer (Repositories, Caching, Identity)
└── test/
│   ├── KingHotelProject.UnitTests/
│       ├── API/
│       ├── Application/
│       └── Infrastructure/


Technologies:

ASP.NET Core 8
C#
SQL
Redis
JWT
Entity Framework Core
Redis
JWT Authentication
FluentValidation
AutoMapper
MediatR
Swagger/OpenAPI
Serilog


Prerequisites :
.NET 8 SDK
MSSQL Server
Redis Server
Visual Studio


Installation :
Clone the repository
git clone: https://github.com/shinsmathew/KingHotelProject.git

Configure the connection strings in appsettings.json:

"ConnectionStrings": {
  "DefaultConnection": "Your_SQL_Server_Connection_String",
  "Redis": "Your_Redis_Connection_String"
}


Configure JWT settings:

"Jwt": {
  "Key": "Your_Secret_Key",
  "Issuer": "KingHotelProject",
  "Audience": "KingHotelProjectClients",
  "ExpiryMinutes": 30
}

Configure Redis Server in you system.


