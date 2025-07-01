# E‑Commerce Application

<!-- badges: start -->

[![Build Status](https://img.shields.io/github/actions/workflow/status/USERNAME/REPO/ci.yml?branch=main)]
[![Build Status](https://img.shields.io/github/actions/workflow/status/USERNAME/REPO/ci.yml?branch=main)]

<!-- badges: end -->

## 🎬 Demo

![Swagger UI screenshot](docs/swagger.png)

## Features

- User authentication and management (registration, login, profile, password reset)
- JWT-based authentication and role-based authorization (admin/client)
- Product catalog with CRUD operations (admin only for create/update/delete)
- Product image upload, update, and deletion (static file serving from `wwwroot/images/products`)
- Shopping cart management (add, remove, view items)
- Order creation, viewing, and pagination
- Payment method selection for orders
- Contact form handling
- Email integration (e.g., SendGrid) for notifications like password reset
- API endpoints documented and testable via Swagger/OpenAPI (with JWT support in Swagger UI)
- Middleware and filters for request/response processing, statistics, and debugging
- Entity Framework Core with SQL Server for data access
- Repository pattern for data abstraction
- DTOs for data transfer between layers
- Secure password hashing using ASP.NET Core Identity
- Use of AutoMapper for object mapping between entities and DTOs
- Modular architecture with separation of concerns (Controllers, BLL, DAL, DTOs, Repositories)
- Environment-based configuration (e.g., `appsettings.Development.json`)
- Static file serving (e.g., product images)
- Error handling and validation for API endpoints
- Pagination support for order and user listings

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

## Requirements

- .NET 8.0 SDK
- SQL Server

## License

MIT
