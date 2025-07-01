# E‑Commerce Application

[![.NET](https://img.shields.io/badge/.NET-8.0-blue?logo=dotnet)]
[![CI](https://img.shields.io/github/actions/workflow/status/USERNAME/REPO/ci.yml?label=CI&branch=main)]
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)]

## 📌 Table of Contents

- [Overview](#overview)
- [Demo](#demo)
- [Features](#features)
- [Getting Started](#getting-started)
- [Usage Examples](#usage-examples)
- [Project Structure](#project-structure)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

A .NET 8 Web API for e-commerce built with EF Core 9 and SQL Server.  
Supports user management, product catalog, shopping cart, orders, payment, and email notifications.

## 🎬 Demo

![App Demo](docs/swagger.png)
_You can replace with a GIF showcasing key flows like product creation, cart, and orders._

## ✅ Features

- User registration, login, profile management, and password reset
- JWT authentication and role-based authorization
- Product catalog CRUD (admin-only for create/update/delete)
- Product image upload and serving from `wwwroot/images/products`
- Shopping cart: add, remove, and list items
- Order creation with pagination and payment method selection
- Contact form endpoint
- Email notifications via SendGrid (password resets, order confirmation)
- Swagger/OpenAPI documentation and JWT support in UI
- Middleware for custom logging, stats, and error handling
- EF Core with repository pattern for data access
- DTOs for clean data transfer between layers
- Secure password hashing using Identity
- AutoMapper integration
- Environment-based config (`appsettings.*.json`)
- Static file hosting
- Validation, error handling, and API-level pagination

## 🚀 Getting Started

```bash
git clone https://github.com/USERNAME/REPO.git
cd REPO
dotnet ef database update
dotnet run
```
