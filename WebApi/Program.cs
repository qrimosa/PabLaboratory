using AppCore.Interfaces;
using AppCore.Module;
using Infrastructure.EntityFramework.Context;
using Infrastructure.Security;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// 1. JWT & Security Configuration
var jwtSettings = new JwtSettings(builder.Configuration);
builder.Services.AddSingleton(jwtSettings);

// 2. Register Modules
// This includes your Repositories, DbContext, and AuthService
builder.Services.AddContactsEfModule(builder.Configuration);
builder.Services.AddContactsModule(builder.Configuration);

// 3. Register JWT Authentication & Authorization Policies
builder.Services.AddJwt(jwtSettings);

// 4. Register Data Seeder
builder.Services.AddScoped<IDataSeeder, IdentityDbSeeder>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<ProblemDetailsExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// 5. Configure Middleware Pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    
    // --- Data Seeding Logic ---
    using var scope = app.Services.CreateScope();
    var seeders = scope.ServiceProvider.GetServices<IDataSeeder>().OrderBy(s => s.Order);
    foreach (var seeder in seeders)
    {
        await seeder.SeedAsync();
    }
}

//app.UseHttpsRedirection();

// IMPORTANT: Authentication must come BEFORE Authorization
app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler();
app.MapControllers();

app.Run();