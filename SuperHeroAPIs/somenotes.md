---
ARCHITECTURE PATTERNS USED IN THIS PROJECT
---

## 1. Layered Architecture (the big picture)

Every HTTP request flows through exactly these layers in order:

    HTTP Request
        ↓
    Controller      → receives the request, calls the service, returns HTTP response
        ↓
    Service         → business logic, maps DTOs to/from entities
        ↓
    Repository      → data access only, talks to EF Core / database
        ↓
    Database (PostgreSQL)

Each layer only knows about the layer directly below it.
The controller never touches the database. The repository never knows about HTTP.

---

## 2. Repository Pattern

Files: Repositories/ISuperHeroRepository.cs + SuperHeroRepository.cs

Wraps all database calls in one place. The rest of the app never calls
EF Core directly — it goes through the repository.

    ISuperHeroRepository   → the contract (what can be done)
    SuperHeroRepository    → the implementation (how it's done via EF Core)

Why: if you switch from PostgreSQL to MongoDB tomorrow, you only rewrite
the repository. The service and controller don't change at all.

---

## 3. Service Layer

Files: Services/ISuperHeroService.cs + SuperHeroService.cs

Sits between the controller and the repository. Responsible for:
- Converting incoming DTOs into entities (to save to DB)
- Converting entities back into DTOs (to return to the caller)
- Any business logic (e.g. checking rules before saving)

    ISuperHeroService   → the contract
    SuperHeroService    → the implementation

The controller only calls the service. It never builds entities or maps data.

---

## 4. Interface + Concrete Class (the I-prefix pattern)

Every service and repository has two files:
  - IClassName  → the interface (just the method signatures, no code)
  - ClassName   → the class that implements those methods

The layers above always depend on the interface, not the class:

    private readonly ISuperHeroRepository _repository;  // not SuperHeroRepository

This means you can swap the real implementation for a fake one (e.g. in-memory
for testing) by changing a single line in Program.cs. Nothing else breaks.

    // production
    builder.Services.AddScoped<ISuperHeroRepository, SuperHeroRepository>();

    // swap for testing
    builder.Services.AddScoped<ISuperHeroRepository, FakeSuperHeroRepository>();

---

## 5. Operation-Specific DTOs (Data Transfer Objects)

Folder: DTOs/

DTOs are the shapes of data that cross the API boundary. The entity (SuperHero)
is never exposed directly — you always map to/from a DTO.

Each controller action has its own DTO:

    SuperHeroGetAllDto   → GET /api/superhero        lightweight, Id + Name only
    SuperHeroGetDto      → GET /api/superhero/{id}   full detail, all fields
    SuperHeroCreateDto   → POST /api/superhero        fields needed to create
    SuperHeroUpdateDto   → PUT  /api/superhero/{id}   fields allowed to update

Why split them instead of one shared DTO:
- GetAll is lightweight on purpose (list views don't need every field)
- Create and Update may have different required fields in real apps
- You can change one without affecting the others

---

## 6. FluentValidation

Package: FluentValidation.AspNetCore
Files: Validators/SuperHeroCreateDtoValidator.cs + SuperHeroUpdateDtoValidator.cs

Validates incoming request DTOs before they reach the controller method.
If validation fails, FluentValidation automatically returns a 400 Bad Request
with error messages — no manual checking needed in the controller.

    RuleFor(x => x.Name)
        .NotEmpty().WithMessage("Name is required.")
        .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

Registered in Program.cs:
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();

---

## 7. Dependency Injection (DI)

Registered in Program.cs:
    builder.Services.AddScoped<ISuperHeroRepository, SuperHeroRepository>();
    builder.Services.AddScoped<ISuperHeroService, SuperHeroService>();

AddScoped means a new instance is created per HTTP request.

When ASP.NET creates SuperHeroController it sees it needs ISuperHeroService,
so it creates SuperHeroService. That needs ISuperHeroRepository, so it creates
SuperHeroRepository. That needs DataContext, which is already registered.
Everything is wired automatically — you never call `new` yourself.

---

Your Code
   ↓
DbContext
   ↓
Entity Framework Core (query + tracking engine)
   ↓
Database Provider (e.g. Microsoft.EntityFrameworkCore.SqlServer)
   ↓
ADO.NET (SqlClient)
   ↓
SQL Server Database


in Controller., - you inject the Service


DB SETUP GUIDE:

ere's the complete setup guide based on exactly what you did:

  ---
  ASP.NET Core Web API + PostgreSQL Setup Guide

  1. Install PostgreSQL (macOS)

  brew install postgresql@17
  brew services start postgresql@17
  Verify it's running:
  pg_isready

  2. Install NuGet Packages

  In your project directory:
  dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
  dotnet add package Microsoft.EntityFrameworkCore.Design
  dotnet add package DotNetEnv

  3. Create the .env File

  In your project folder (same level as Program.cs):
  ConnectionStrings__DefaultConnection=Host=localhost;Port=5432;Data
  base=YourDBName;Username=postgres;Password=postgres
  
  4. Add .env to .gitignore

  .env

  5. Create the DataContext
  
  Data/DataContext.cs:
  using Microsoft.EntityFrameworkCore;

  namespace YourProject.Data;

  public class DataContext : DbContext
  {
      public DataContext(DbContextOptions<DataContext> options) :
  base(options) { }
 
      public DbSet<YourEntity> YourEntities { get; set; }
  }
  
  6. Update appsettings.json

  Leave the connection string blank (.env overrides it):
  "ConnectionStrings": {
    "DefaultConnection": ""
  }

  7. Update Program.cs

  using DotNetEnv;
  using Microsoft.EntityFrameworkCore;
  using YourProject.Data;

  Env.Load();

  var builder = WebApplication.CreateBuilder(args);

  builder.Services.AddDbContext<DataContext>(options =>
      options.UseNpgsql(builder.Configuration.GetConnectionString("D
  efaultConnection")));

  8. Run Migrations

  dotnet ef migrations add Initial
  dotnet ef database update

  ---
  That's it. Your database and tables will be created in PostgreSQL
  automatically after step 8.