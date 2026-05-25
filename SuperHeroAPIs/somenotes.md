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