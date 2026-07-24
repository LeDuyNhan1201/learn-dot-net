# Restaurant Management System (.NET 10)

A modular Restaurant Management System built with **.NET 10**, following **Clean Architecture**, principles, and **Vertical Slice Architecture**.

The primary goal of this project is **learning and experimenting with enterprise-grade architecture**, rather than building the simplest possible CRUD application.

---

# Project Structure

```
Backend
├── BuildingBlocks
├── Modules
│   └── Restaurant
└── Backend.slnx
```

## BuildingBlocks

Shared libraries that provide reusable infrastructure and cross-cutting concerns.

| Project | Responsibility |
|----------|---------------|
| **BuildingBlocks.SharedKernel** | Shared DTOs, common models, helper classes, localization, common errors, utilities |
| **BuildingBlocks.Domain** | Base entities, domain events, repositories, contracts, execution context |
| **BuildingBlocks.Application** | MediatR pipeline behaviors, validators, data seeders, REST clients |
| **BuildingBlocks.Persistence** | EF Core integration, repositories, DbContexts, interceptors, database configuration |
| **BuildingBlocks.Identity** | Keycloak integration, authentication, authorization, JWT/OIDC support |
| **BuildingBlocks.Messaging** | Messaging abstractions and integrations (MassTransit, etc.) |
| **BuildingBlocks.Observability** | OpenTelemetry, metrics, logging, tracing, monitoring configuration |
| **BuildingBlocks.OpenApi** | OpenAPI/Swagger/Scalar configuration |
| **BuildingBlocks.API** | Shared Minimal API infrastructure, middleware, endpoint abstractions |
| **BuildingBlocks.Testing** | Shared testing utilities, fixtures, builders and assertions |

---

## Restaurant Module

Business logic is organized as a vertical module.

| Project | Responsibility |
|----------|---------------|
| **Restaurant.Domain** | Domain models, business rules, aggregates, repositories |
| **Restaurant.Application** | Commands, queries, handlers, validators |
| **Restaurant.Infrastructure** | Database implementation, repository implementations, external services |
| **Restaurant.API** | Minimal API endpoints and module configuration |

---

# Features

## Menu Management

Manage restaurant menu items.

- Create, retrieve, update and delete menu items
- Search menu items by keyword
- Pagination support
- Menu categories
    - Food
        - Breakfast
        - Lunch
        - Dinner
    - Drinks
        - Soft Drinks
        - Alcoholic Beverages

Each menu item contains:

- Name
- Description
- Image
- Price
- Notes

---

## Bill Management

Manage customer orders.

Features include:

- Create bills
- Retrieve bills
- Update bills
- Delete bills
- Add or remove menu items
- Update item quantities

Bill details include:

- Ordered menu items
- Quantity
- Ordered time
- Total price

---

# Architecture

The project combines several architectural patterns:

- Clean Architecture
- Vertical Slice Architecture
- Repository Pattern
- Dependency Injection
- Minimal APIs

---

# Why This Project Is Intentionally Over-Engineered

This project is designed as a learning playground for enterprise application architecture.

The goals are:

- Learn enterprise-grade project organization
- Practice Clean Architecture
- Explore reusable Building Blocks
- Build a reusable template for future .NET projects
- Evaluate scalability and maintainability of modular applications

---

# Current Progress

## Foundation

- Modular solution structure
- Clean Architecture
- Vertical Slice Architecture
- Dependency Injection
- Global exception handling

## Domain

- Base Entity
- Auditable Entity
- Repository Pattern

## Application

- MediatR
- FluentValidation pipeline
- Request validation behavior

## Persistence

- Entity Framework Core
- Auditing interceptor
- Repository implementation

## Security

- Keycloak integration
- OpenID Connect (OIDC)
- JWT authentication

## API

- Minimal APIs
- OpenAPI
- Scalar API documentation

## Internationalization

- Localization using `IStringLocalizer`

## Observability

- OpenTelemetry
- Metrics
- Distributed tracing
- Structured logging

## Messaging

- MassTransit integration
- Outbox pattern

---

# Technology Stack

- .NET 10
- ASP.NET Core Minimal APIs
- Entity Framework Core
- PostgreSQL
- MediatR
- FluentValidation
- Keycloak
- OpenTelemetry
- Scalar
- Docker
- MassTransit (planned)

---