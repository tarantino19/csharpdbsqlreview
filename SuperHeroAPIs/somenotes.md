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

  -----


  
---
AUTHENTICATION & AUTHORIZATION LESSONS
---

## 8. Authentication vs Authorization

Authentication  — WHO are you?      (happens at login — verify identity, issue token)
Authorization   — WHAT can you do?  (happens every request — check if token allows access)

Analogy: Authentication is showing your ID at the door.
         Authorization is whether your ID lets you into the VIP section.

In ASP.NET:
    app.UseAuthentication()  → reads the JWT, populates User claims ("I know who you are")
    app.UseAuthorization()   → checks [Authorize] attributes ("I know what you can do")

---

## 9. JWT Auth Flow (the standard pattern)

    POST /auth/register  →  hash password, save user to DB
    POST /auth/login     →  check password → generate JWT → return token to client
    POST /auth/logout    →  invalidate refresh token in DB (for stateless JWT, client just discards it)

Once the client has the token, it sends it on every request:
    Authorization: Bearer <token>

ASP.NET middleware validates it automatically — you never write that validation logic yourself.

---

## 10. Protected Routes — [Authorize] and [AllowAnonymous]

These are C# attributes from built-in ASP.NET namespaces:

    using Microsoft.AspNetCore.Mvc;           // [ApiController], [Route]
    using Microsoft.AspNetCore.Authorization; // [Authorize], [AllowAnonymous]

No extra NuGet packages needed. Usage:

    [Authorize]               // entire controller is protected
    public class HeroesController : ControllerBase
    {
        [AllowAnonymous]      // override — this one endpoint is public
        public IActionResult PublicEndpoint() { ... }

        [Authorize(Roles = "Admin")]  // role-based access
        public IActionResult AdminOnly() { ... }
    }

If JWT is missing/invalid/expired → 401 Unauthorized returned automatically.
Your action method never even runs.

---

## 11. Why You Still Build AuthController + AuthService

ASP.NET provides the *checking* mechanism — not the user data or token generation.

    ASP.NET gives you (free):
    - Middleware that validates JWT on incoming requests
    - [Authorize] attribute that blocks unauthorized requests
    - UserManager / SignInManager for password hashing

    ASP.NET does NOT do for you:
    - Know your users (your DB, your schema)
    - Generate a JWT and return it to the client
    - Define your login/register HTTP endpoints

    So you build:
    - AuthController  → your login/register/logout HTTP endpoints
    - AuthService     → generates JWT, validates credentials, talks to DB

    ASP.NET handles enforcement. You handle issuance.

Follow the same layered pattern as the rest of the app:
    AuthController → IAuthService → AuthService → (UserManager / DB)

Register in Program.cs like any other service:
    builder.Services.AddScoped<IAuthService, AuthService>();
