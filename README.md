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

## Key Features in Detail

- **Caching**: Products are cached in Redis for improved performance
- **Email Notifications**: Async order confirmations via RabbitMQ using MailKit
- **Validation**: Request validation before processing
- **Error Handling**: Standardized error responses
- **Logging**: Comprehensive logging with Serilog to console and file
