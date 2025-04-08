King Hotel Project - REST API
--------------------------------------------------------------------------------------------------------------
 
A robust hotel management system API built with ASP.NET Core 8, implementing modern architectural practices, secure authentication, Redis caching, and a clean separation of concerns.

✨ Features
--------------------------------------------------------------------------------------------------------------
🔐 Authentication & Authorization
Secure JWT-based authentication
Role-based access control (Admin/Staff)

🏨 Hotel Management
Full CRUD operations
Bulk hotel creation support

🍽️ Dish Management
Full CRUD operations
Bulk dish creation support

⚡ Redis Caching
Enhanced performance with Redis as a caching layer

🛡️ Validation & Error Handling
FluentValidation for request validation
Custom middleware for global exception handling with detailed error responses

📦 Clean Architecture
DTO-to-Entity mapping using AutoMapper
CQRS pattern implementation via MediatR

📘 API Documentation
Swagger UI with JWT token support

--------------------------------------------------------------------------------------------------------------
🏗️ Project Structure
--------------------------------------------------------------------------------------------------------------
KingHotelProject/
├── src/
│   ├── KingHotelProject.API/            # API layer (Controllers, Middleware)
│   ├── KingHotelProject.Core/           # Domain layer (Entities, Interfaces, Exceptions)
│   ├── KingHotelProject.Application/    # Application layer (DTOs, Commands, Queries, Validators)
│   ├── KingHotelProject.Infrastructure/ # Infrastructure (Repositories, Caching, Identity)
└── test/
    └── KingHotelProject.UnitTests/
        ├── API/
        ├── Application/
        └── Infrastructure/

--------------------------------------------------------------------------------------------------------------

🛠️ Tech Stack
--------------------------------------------------------------------------------------------------------------
Framework: ASP.NET Core 8
Language: C#
Database: SQL Server
Caching: Redis
Authentication: JWT
ORM: Entity Framework Core
Validation: FluentValidation
Mapping: AutoMapper
CQRS: MediatR
Logging: Serilog
Documentation: Swagger / OpenAPI

✅ Prerequisites
--------------------------------------------------------------------------------------------------------------
Before running the project, ensure you have:

.NET 8 SDK
MSSQL Server
Redis Server
Visual Studio 2022+ or any preferred IDE

--------------------------------------------------------------------------------------------------------------

🚀 Getting Started
--------------------------------------------------------------------------------------------------------------
Clone the Repository
git clone https://github.com/shinsmathew/KingHotelProject.git

Configure App Settings
Update your appsettings.json file with the correct values:

"ConnectionStrings": {
  "DefaultConnection": "Your_SQL_Server_Connection_String",
  "Redis": "Your_Redis_Connection_String"
},

"Jwt": {
  "Key": "Your_Secret_Key",
  "Issuer": "KingHotelProject",
  "Audience": "KingHotelProjectClients",
  "ExpiryMinutes": 30
}

Ensure Redis is Running
Install and run Redis on your system
Make sure the Redis connection string matches your environment setup

Run the Application
Build and run the solution from Visual Studio or use the .NET CLI:

