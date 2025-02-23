# ECommerceAPI

A modern e-commerce API built with .NET 6.0 using clean architecture principles, CQRS pattern, and event-driven design.

## Features

- **Product Management**: Browse and search products with category filtering
- **Order Processing**: Create orders with email notifications
- **Caching**: Implemented caching strategy with Redis
- **Background Services**: Asynchronous email sending via RabbitMQ
- **Clean Architecture**: Organized in layers (API, Application, Core, Infrastructure)
- **CQRS Pattern**: Using MediatR for command and query separation
- **Validation**: Request validation using FluentValidation
- **Error Handling**: Global exception handling with standardized responses
- **Logging**: Comprehensive logging with Serilog

## Tech Stack

- .NET 6.0
- Entity Framework Core 6.0
- MySQL (Pomelo.EntityFrameworkCore.MySql)
- MediatR
- FluentValidation
- RabbitMQ
- AutoMapper
- Redis Cache
- Serilog
- MailKit

## Project Structure

```
ECommerceAPI/
├── ECommerceAPI.API           # API Controllers, Middleware
├── ECommerceAPI.Application   # Commands, Queries, DTOs
├── ECommerceAPI.Core         # Entities, Interfaces
└── ECommerceAPI.Infrastructure # Implementations, Services
```

## Getting Started

1. **Prerequisites**
   - .NET 6.0 SDK
   - MySQL Server (Port: 3308)
   - RabbitMQ Server
   - Redis Server (Port: 6380)

2. **Configuration**
   - Update database connection in `appsettings.json`:
     ```json
     "ConnectionStrings": {
       "Mobiliva-DB": "Server=localhost;Port=3308;Database=MobilivaDb;User Id=root;Password=Password1*;"
     }
     ```
   - Configure RabbitMQ and Redis settings
   - Set up email configuration for notifications

3. **Run Migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run the API**
   ```bash
   dotnet run --project ECommerceAPI.API
   ```

## API Endpoints

- `GET /api/products` - Get all products (optional category filter)
- `POST /api/orders` - Create a new order

## Key Features in Detail

- **Caching**: Products are cached in Redis for improved performance
- **Email Notifications**: Async order confirmations via RabbitMQ using MailKit
- **Validation**: Request validation before processing
- **Error Handling**: Standardized error responses
- **Logging**: Comprehensive logging with Serilog to console and file