using AppCore.Interfaces;
using Infrastructure.Memory;

var builder = WebApplication.CreateBuilder(args);

// 1. Register Repositories (Data Layer)
// We use Singleton because the data is stored in Dictionary fields within these classes.
builder.Services.AddSingleton<IPersonRepository, MemoryPersonRepository>();
builder.Services.AddSingleton<ICompanyRepository, MemoryCompanyRepository>();
builder.Services.AddSingleton<IOrganizationRepository, MemoryOrganizationRepository>();

// 2. Register Unit of Work (Coordination Layer)
builder.Services.AddSingleton<IContactUnitOfWork, MemoryContactUnitOfWork>();

// 3. Register Application Services (Business Logic Layer)
builder.Services.AddSingleton<IPersonService, MemoryPersonService>();

// 4. Add Controllers support (needed for your ContactsController)
builder.Services.AddControllers();

// OpenAPI/Swagger Setup
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 5. Map Controller Routes
app.MapControllers();

app.Run();