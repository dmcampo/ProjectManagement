# Project Management - .NET Clean Architecture

This is a complete, monolithic .NET 8 application following Clean Architecture principles. It provides an API and an MVC web frontend for managing projects and tasks. Both Web MVC and API applications share the exact same Application, Domain, and Infrastructure layers.

## Features
- **Clean Architecture**: Separation of concerns into Domain, Application, Infrastructure, Web, and API.
- **Shared Business Logic**: Centralized use cases with `ProjectService`, `TaskService`, and `AuthService`.
- **Database**: PostgreSQL via Supabase using Entity Framework Core.
- **REST API**: Swagger/OpenAPI configured, secured with JWT Bearer authentication.
- **Web MVC**: Server-rendered UI using Bootstrap 5, secured with Cookie Authentication.
- **Validation and Error Handling**: Global custom exception middleware.
- **Tests**: Contains unit tests for critical business logic using xUnit, Moq, and FluentAssertions.

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Supabase](https://supabase.com) PostgreSQL database (or local PostgreSQL instance).

## Configuration
Before running the application, ensure you have set up your database connection string and JWT settings.
The configurations use placeholders that you should replace in `appsettings.json` (or `appsettings.Development.json`):

```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=<SUPABASE_HOST>;Port=<SUPABASE_PORT>;Database=<SUPABASE_DB_NAME>;Username=<SUPABASE_DB_USER>;Password=<SUPABASE_DB_PASSWORD>;Ssl Mode=Prefer"
  },
  "JwtSettings": {
    "Secret": "<JWT_SECRET>",
    "Issuer": "<JWT_ISSUER>",
    "Audience": "<JWT_AUDIENCE>",
    "ExpirationInMinutes": 120
  }
```

> **Note:** Do not hardcode production secrets. Use User Secrets or continuous deployment environment variables for sensitive keys.

## Database setup (Migrations)
EF Core code-first migrations are used for the database schema.
To apply migrations and build the database schema in Supabase, execute:

```bash
cd ProjectManagement.Api
dotnet tool install --global dotnet-ef  # if not already installed
dotnet ef database update --project ../ProjectManagement.Infrastructure --startup-project .
```

*Alternatively, you can just run `dotnet run` on the API project if you add `context.Database.Migrate()` in `Program.cs`.*

## Running the projects

### REST API
To run the REST API and access Swagger UI:
```bash
cd ProjectManagement.Api
dotnet restore
dotnet run
```
Navigate to `https://localhost:<port>/swagger` to test the API endpoints. 

### MVC Web App
To run the Web application:
```bash
cd ProjectManagement.Web
dotnet restore
dotnet run
```
Navigate to `https://localhost:<port>`. It will redirect you to the login page if you are unauthenticated.
> **Note:** The MVC App directly consumes the Application Services (use cases). It does not perform HTTP calls to the API. 

## Testing
To run the unit tests:
```bash
dotnet test
```

## Technical Decisions
- **Filter by Status instead of Priority for Projects:** The task requirement mentions filtering projects by `Priority`, but the `Project` entity doesn't contain a Priority field. A logical decision was made to filter projects by `Status` instead.
- **Task Orders and Recompaction:** When a new task is added without a specific order, it gets appended to the end. When a task is deleted, the remaining tasks are recompacted to maintain an elegant contiguous `Order` (e.g., 1, 2, 3, 4).
- **Authentication**: A custom `User` table is used with `BCrypt.Net-Next` instead of Identity to keep the persistence light, clean, and database-provider agnostic, while remaining highly secure. 

## Test User Credentials
To create your first user, simply navigate to the Register view in MVC or call `POST /api/auth/register` via Swagger. 
Alternatively, use the following sample to seed a test user on your database:
```sql
-- Will be created upon your registration
Email:  [admin@admin.com]
Password: [admin123]
```
