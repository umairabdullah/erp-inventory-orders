# ERP Inventory & Orders API

![CI](https://github.com/umairabdullah/erp-inventory-orders/actions/workflows/ci.yml/badge.svg)

A backend for small-business inventory and order management, built with ASP.NET Core using onion architecture and domain-driven design.

## Features

- Categories, products, and stock levels with a full stock-movement history (who changed stock, when, and why)
- Customers and orders with a status flow: Pending ? Confirmed ? Shipped ? Delivered, or Cancelled
- Orders reduce stock in a single transaction, and cancelling an order returns the stock
- Authentication: JWT access tokens, rotating refresh tokens with reuse detection, and email confirmation
- Roles (Admin, Manager, Staff) with ownership rules, for example Staff can only cancel their own pending orders
- Optimistic concurrency, so two people cannot oversell the last units
- Confirmation emails (account and order) sent through domain events
- Rate limiting, CORS, structured logging (Serilog), and health checks

## Architecture

```
Erp.Api             Controllers, middleware, DI setup          (outer ring)
Erp.Infrastructure  EF Core, repositories, JWT, SMTP           (outer ring)
Erp.Application     Use cases (MediatR), validators, DTOs      (inner ring)
Erp.Domain          Entities, value objects, business rules    (center)
```

Dependencies only point inward. The domain has no references to any other project, and commands (writes) go through the domain while queries (reads) project straight into DTOs.

## Tech stack

ASP.NET Core 10, EF Core 10, SQL Server 2022, MediatR 12, FluentValidation, Serilog, MailKit, xUnit, Docker.

## Run with Docker

1. Copy `.env.example` to `.env` and set your own values.
2. Start everything:

   ```
   docker compose up --build
   ```

3. Open:
   - Swagger UI: http://localhost:8080/swagger
   - Mail inbox (smtp4dev): http://localhost:5051
   - Health: http://localhost:8080/health/ready

Sign in as the admin from your `.env` (`POST /api/Auth/login`), then click Authorize in Swagger and paste the access token.

## Run locally

Requires the .NET 8 SDK and SQL Server.

```
dotnet user-secrets init --project Src/Api/Erp.Api
dotnet user-secrets set "Jwt:SigningKey" "<random key, 32+ characters>" --project Src/Api/Erp.Api
dotnet user-secrets set "Seed:AdminEmail" "admin@example.com" --project Src/Api/Erp.Api
dotnet user-secrets set "Seed:AdminPassword" "<your password>" --project Src/Api/Erp.Api
dotnet ef database update --project Src/Infrastructure/Erp.Infrastructure --startup-project Src/Api/Erp.Api
dotnet run --project Src/Api/Erp.Api
```

## Tests

```
dotnet test
```

## Known limitations

- Access tokens last 15 minutes, so a role change applies to existing tokens only after they expire.
- Rate limiting is in memory, so it works for a single instance only.
- Migrations run at startup when `Database__MigrateOnStartup` is true, which suits a demo more than a large deployment.
