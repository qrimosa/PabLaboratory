using AppCore.Authorization;
using AppCore.Interfaces;
using AppCore.Services;
using Infrastructure.EntityFramework.Context;
using Infrastructure.EntityFramework.Entities;
using Infrastructure.EntityFramework.Repositories;
using Infrastructure.EntityFramework.UnitOfWork;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure;

public static class ContactsInfrastructureModule
{
    public static IServiceCollection AddContactsEfModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // 1. Repositories & Services
        services.AddScoped<IPersonRepository, EfPersonRepository>();
        services.AddScoped<ICompanyRepository, EfCompanyRepository>();
        services.AddScoped<IOrganizationRepository, EfOrganizationRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPersonService, PersonService>();

        // 2. Unit of Work
        services.AddScoped<IContactUnitOfWork, EfContactsUnitOfWork>();

        // 3. Database Context (SQLite)
        services.AddDbContext<ContactsDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("CrmDb")));

        // 4. Identity Setup
        services.AddIdentity<CrmUser, CrmRole>(options =>
            {
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ContactsDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection AddJwt(this IServiceCollection services, JwtSettings jwtOptions)
    {
        // Explicitly capture values to prevent late-binding null issues
        var issuer = jwtOptions.Issuer;
        var audience = jwtOptions.Audience;
        var signingKey = jwtOptions.GetSymmetricKey();

        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
                
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        System.Diagnostics.Debug.WriteLine($"JWT Auth Failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization(options =>
        {
            // Using nameof ensures these match your [Authorize(Policy = nameof(...))] attributes
            
            options.AddPolicy(nameof(CrmPolicies.AdminOnly), policy =>
                policy.RequireRole("Administrator"));

            options.AddPolicy(nameof(CrmPolicies.SalesAccess), policy =>
                policy.RequireRole("Administrator", "SalesManager", "Salesperson"));

            options.AddPolicy(nameof(CrmPolicies.ReadOnlyAccess), policy =>
                policy.RequireRole("Administrator", "SalesManager", "Salesperson", "SupportAgent", "ReadOnly"));

            options.AddPolicy(nameof(CrmPolicies.ActiveUser), policy =>
                policy.RequireAuthenticatedUser()
                    .RequireClaim("status", "Active"));
        });

        return services;
    }
}