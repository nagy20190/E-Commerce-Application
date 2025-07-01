# E-CommerceApplication

This is an ASP.NET Core Web API E-Commerce Application.

## Features

- Product image upload, update, and deletion (with static file serving from wwwroot/images/products)
- Role-based authorization (admin/client) for sensitive operations (e.g., product management)
- User authentication and management
- Product catalog
- Shopping cart
- Order processing
- Contact form
- JWT-based authentication and authorization
- User registration, login, profile management, and password reset
- Admin and client user roles
- Product catalog with CRUD operations
- Shopping cart management (add, remove, view items)
- Order creation, viewing, and pagination
- Payment method selection for orders
- Contact form handling
- Email sending (for password reset, etc.)
- API endpoints documented with Swagger/OpenAPI
- Entity Framework Core with SQL Server for data access
- Repository pattern for data abstraction
- DTOs for data transfer between layers
- Static file serving (e.g., product images)
- Secure password hashing
- Pagination support for order and user listings
- Error handling and validation for API endpoints
- Modular architecture with separation of concerns (Controllers, BLL, DAL, DTOs, Repositories)
- Middleware and filters for request statistics and debugging
- Environment-based configuration (e.g., appsettings.Development.json)

## Getting Started

1. Clone the repository.
2. Open the solution in Visual Studio.
3. Update the database connection string in `appsettings.json`.
4. Run database migrations.
5. Start the application.

## Project Structure

- `E-CommerceApplication/` - Main web API project
- `E-CommerceApplication.BLL/` - Business logic layer
- `E-CommerceApplication.DAL/` - Data access layer

Tech Stack/Dependencies:
ASP.NET Core 8.0
Entity Framework Core 9.0 (with SQL Server)
Swashbuckle/Swagger for API docs
AutoMapper for mapping
SendGrid for email
JWT for authentication

## Requirements

- .NET 8.0 SDK
- SQL Server
