## Insurance Management System - Production-Ready Architecture Implementation

### COMPLETED: Users Module (Full CQRS Implementation)

This document summarizes the production-ready ASP.NET Core Web API implementation using Clean Architecture, CQRS, and best practices.

---

## Architecture Overview

### Layer Structure

1. **Domain Layer** (`Insurance.Domain`)
   - Pure business logic with NO external dependencies
   - Entities with soft-delete support via `BaseEntity`
   - Domain interfaces (repositories)

2. **Application Layer** (`Insurance.Application`)
   - CQRS commands/queries
   - Handlers for commands and queries
   - DTOs for request/response
   - Validators using FluentValidation
   - AutoMapper profiles
   - Result<T> pattern for responses
   - Pagination support

3. **Persistence Layer** (`Insurance.Persistence`)
   - Entity Framework Core DbContext
   - Generic and specific repository implementations
   - Unit of Work pattern for transaction management
   - Database migrations

4. **API Layer** (`Insurance.API`)
   - REST controllers (thin layer, no business logic)
   - Global exception handling middleware
   - Swagger/OpenAPI configuration
   - Dependency injection setup

---

## Key Implementation Patterns

### 1. Result<T> Pattern
- No exceptions thrown from handlers
- All responses wrapped in Result<T> with success/failure states
- Location: `Insurance.Application/Common/Models/Result.cs`

### 2. Repository Pattern
- Generic `IRepository<T>` with soft-delete support
- Specific repositories extend generic (e.g., `IUserRepository`)
- All queries exclude `IsDeleted = true` by default
- Pagination support built-in

### 3. Unit of Work Pattern
- Coordinates multiple repositories
- Atomic transaction management
- Repositories don't call `SaveChangesAsync` individually
- Location: `Insurance.Persistence/UnitOfWork/IUnitOfWork.cs`

### 4. CQRS with MediatR
- Separate command and query handlers
- Each handler has dedicated validator via FluentValidation
- Pipeline behavior validates requests before handlers execute
- Auto-registration via reflection

### 5. Soft Delete
- All entities inherit from `BaseEntity` with `IsDeleted` flag
- No hard deletes - data preserved for audit/recovery
- Queries automatically filter deleted records

### 6. Pagination
- `PaginatedResult<T>` with metadata (page, pageSize, totalItems, totalPages)
- `PaginationParams` for query parameters
- Location: `Insurance.Application/Common/Models/PaginatedResult.cs`

---

## Users Module - Complete Implementation

### Domain Layer

**Entity**: `Insurance.Domain/Entities/User.cs`
- Inherits from `BaseEntity` (provides Id, CreatedAt, UpdatedAt, IsDeleted)
- Properties: Username, Email, PasswordHash, FirstName, LastName, PhoneNumber, Role, IsActive, LastLoginAt
- Required constructor for dependency injection with EF Core

**Enums**: `Insurance.Domain/Enums/UserRole.cs`
- Defined roles: Admin, Agent, Customer, etc.

**Repository Interface**: `Insurance.Domain/Interfaces/IUserRepository.cs`
- Extends `IRepository<User>`
- User-specific methods:
  - `GetByUsernameAsync(string username, CancellationToken)`
  - `GetByEmailAsync(string email, CancellationToken)`
  - `SearchByNameAsync(string searchTerm, CancellationToken)`
  - `GetByRoleAsync(UserRole role, CancellationToken)`
  - `UsernameExistsAsync(string username, CancellationToken)`
  - `EmailExistsAsync(string email, CancellationToken)`

### Application Layer

**DTOs**: `Insurance.Application/Features/Users/DTOs/UserDtos.cs`
- `CreateUserDto` - for user registration (includes plain-text password)
- `UpdateUserDto` - for profile updates (excludes password)
- `UserResponseDto` - safe for API responses (excludes password hash)
- `UserListDto` - lightweight DTO for list endpoints

**Commands**: `Insurance.Application/Features/Users/Commands/UserCommands.cs`
- `CreateUserCommand` - create new user
- `UpdateUserCommand` - update user profile
- `DeleteUserCommand` - soft-delete user

**Queries**: `Insurance.Application/Features/Users/Queries/UserQueries.cs`
- `GetUserByIdQuery` - retrieve single user
- `GetAllUsersQuery` - paginated user list with filtering/sorting
- `SearchUsersByNameQuery` - search by first/last name
- `GetUsersByRoleQuery` - filter by role
- `CheckUsernameAvailabilityQuery` - check if username taken
- `CheckEmailAvailabilityQuery` - check if email taken

**Validators**: `Insurance.Application/Features/Users/Validators/UserValidators.cs`
- `CreateUserCommandValidator` - validates username, email, password strength, names
- `UpdateUserCommandValidator` - validates optional fields
- `DeleteUserCommandValidator` - validates user ID
- Query validators for pagination, searches

**Handlers**:
- `CommandHandlers/UserCommandHandlers.cs`
  - `CreateUserCommandHandler` - bcrypt password hashing, uniqueness checks
  - `UpdateUserCommandHandler` - partial updates
  - `DeleteUserCommandHandler` - soft deletion
- `QueryHandlers/UserQueryHandlers.cs`
  - `GetUserByIdQueryHandler`
  - `GetAllUsersQueryHandler` - handles pagination, filtering, sorting
  - Search and availability handlers

**AutoMapper Profile**: `Insurance.Application/Features/Users/MappingProfiles/UserMappingProfile.cs`
- Entity ↔ DTO mappings
- Computed properties (FullName)
- Excludes sensitive fields (PasswordHash)

### Persistence Layer

**Repository Implementation**: `Insurance.Persistence/Repositories/UserRepository.cs`
- Implements `IUserRepository`
- Uses LINQ with `.AsNoTracking()` for queries
- Case-insensitive username/email searches
- All queries exclude soft-deleted users

**Generic Repository**: `Insurance.Persistence/Repositories/GenericRepository.cs`
- Implements `IRepository<T>`
- Common CQRS operations for all entities
- Pagination with `GetPaginatedAsync()`
- Soft delete via `DeleteAsync()`

**Unit of Work**: `Insurance.Persistence/UnitOfWork/IUnitOfWork.cs`
- Coordinates repositories
- Transaction management
- Lazy-loads repositories on first access

### API Layer

**Controller**: `Insurance.API/Controllers/UsersController.cs`
- All endpoints dispatch via MediatR (no business logic)
- RESTful routes under `/api/v1/users/`
- Endpoints:
  - `POST /api/v1/users/create` - create user (201 Created)
  - `GET /api/v1/users/get/{id}` - get by ID
  - `GET /api/v1/users/list` - paginated list with filters
  - `PUT /api/v1/users/update/{id}` - update user
  - `DELETE /api/v1/users/delete/{id}` - delete user (204 No Content)
  - `GET /api/v1/users/search?searchTerm=...` - search by name
  - `GET /api/v1/users/by-role/{role}` - get by role
  - `GET /api/v1/users/check-username/{username}` - availability check
  - `GET /api/v1/users/check-email/{email}` - availability check

- Response format:
  ```json
  {
    "success": true,
    "data": { /* entity DTO */ },
    "pagination": { /* if applicable */ }
  }
  ```

---

## Dependency Injection Configuration

### Application Layer (`Insurance.Application/DependencyInjection.cs`)
```csharp
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
services.AddAutoMapper(Assembly.GetExecutingAssembly());
```

### Persistence Layer (`Insurance.Persistence/DependencyInjection.cs`)
```csharp
services.AddDbContext<InsuranceDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions => {
        sqlOptions.MigrationsAssembly(typeof(DependencyInjection).Assembly.FullName);
        sqlOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(30), null);
    }));
services.AddScoped<IUnitOfWork, UnitOfWork>();
services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
services.AddScoped<IUserRepository, UserRepository>();
```

### Program.cs
```csharp
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
```

---

## Key Features Implemented

### Security
✅ Bcrypt password hashing (via BCrypt.Net-Next)
✅ Username/email uniqueness validation
✅ DTO layer prevents exposing password hash
✅ Nullable reference types enabled
✅ Input validation with FluentValidation

### Data Access
✅ Soft delete support
✅ Pagination with metadata
✅ Async/await throughout
✅ `AsNoTracking()` for read-only queries
✅ Case-insensitive searches
✅ Transaction management via Unit of Work

### API Design
✅ RESTful routes with versioning (/api/v1/)
✅ Proper HTTP status codes (201, 204, 400, 404, 500)
✅ Consistent response format
✅ Swagger/OpenAPI documentation
✅ Query parameter validation

### Code Quality
✅ Clean Architecture (strict layer separation)
✅ CQRS pattern
✅ Dependency Injection
✅ Comprehensive XML documentation
✅ No business logic in controllers
✅ Result pattern for error handling
✅ Cancellation tokens on all async operations

---

##  Required Packages

```xml
<!-- Domain & Application -->
<MediatR Version="12.1.1" />
<FluentValidation Version="11.9.0" />
<AutoMapper Version="13.0.1" />

<!-- Persistence -->
<EntityFrameworkCore Version="8.0.0" />
<EntityFrameworkCore.SqlServer Version="8.0.0" />

<!-- Security -->
<BCrypt.Net-Next Version="4.0.3" />

<!-- API -->
<Swashbuckle.AspNetCore Version="6.0.0" />
```

---

## Next Steps: Implementing Other Entities

To implement **Customers**, **InsuranceProducts**, **Policies**, **Claims**, and **Payments**, follow this exact pattern:

### 1. Domain Layer
- Create entity inheriting from `BaseEntity`
- Create entity-specific enums if needed
- Create `IXxxRepository` extending `IRepository<Xxx>`

### 2. Persistence Layer
- Create `XxxRepository` implementing `IXxxRepository`
- Register in DI: `services.AddScoped<IXxxRepository, XxxRepository>();`
- Update `IUnitOfWork` to expose `IXxxRepository Xxxs { get; }`

### 3. Application Layer
- Create DTOs: `CreateXxxDto`, `UpdateXxxDto`, `XxxResponseDto`, `XxxListDto`
- Create commands: `CreateXxxCommand`, `UpdateXxxCommand`, `DeleteXxxCommand`
- Create queries: `GetXxxByIdQuery`, `GetAllXxxsQuery`, etc.
- Create validators for each command/query
- Create handlers for each command/query
- Create AutoMapper profile with mappings

### 4. API Layer
- Create `XxxsController` with all CRUD endpoints
- Follow same pattern as `UsersController`

---

## Build & Run Commands

```bash
# Restore packages
dotnet restore

# Build solution
dotnet build

# Run migrations (from Persistence project)
dotnet ef migrations add InitialCreate -p Insurance.Persistence
dotnet ef database update -p Insurance.Persistence

# Run API
dotnet run --project Insurance.API

# Access Swagger UI
https://localhost:5001/swagger
```

---

##Common Patterns

### Get Paginated Data in Handler
```csharp
var (items, totalCount) = await _unitOfWork.Users.GetPaginatedAsync(
    request.PageNumber, 
    request.PageSize, 
    cancellationToken);
    
var userDtos = _mapper.Map<List<UserListDto>>(items);
var result = new PaginatedResult<UserListDto>(
    userDtos, 
    request.PageNumber, 
    request.PageSize, 
    totalCount);
    
return Result<PaginatedResult<UserListDto>>.Success(result);
```

### Validate Uniqueness in Handler
```csharp
var exists = await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken);
if (exists)
    return Result<UserResponseDto>.Failure($"Email already registered");
```

### Hash Password Before Saving
```csharp
var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
var user = new User(username, email, passwordHash, firstName, lastName, role, phone);
```

### Return Appropriate HTTP Status
```csharp
if (!result.IsSuccess)
    return result.Data is null ? NotFound(...) : BadRequest(...);
    
return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
```

---

This implementation provides a solid foundation for a production-grade insurance management system with full CQRS support, comprehensive validation, proper error handling, and clean architecture principles.
