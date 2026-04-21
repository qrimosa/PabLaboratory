using AppCore.Interfaces;
using Infrastructure.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class IdentityDbSeeder : IDataSeeder
{
    public int Order => 1;

    private readonly UserManager<CrmUser> _userManager;
    private readonly RoleManager<CrmRole> _roleManager;
    private readonly ILogger<IdentityDbSeeder> _logger;

    public IdentityDbSeeder(
        UserManager<CrmUser> userManager,
        RoleManager<CrmRole> roleManager,
        ILogger<IdentityDbSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        await SeedRolesAsync();
        await SeedUsersAsync();
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new CrmRole(UserRole.Administrator.ToString(), "Pełny dostęp do systemu."),
            new CrmRole(UserRole.SalesManager.ToString(), "Zarządzanie zespołem sprzedaży."),
            new CrmRole(UserRole.Salesperson.ToString(), "Obsługa klientów i szans sprzedaży."),
            new CrmRole(UserRole.SupportAgent.ToString(), "Obsługa zgłoszeń serwisowych."),
            new CrmRole(UserRole.ReadOnly.ToString(), "Tylko odczyt danych.")
        };

        foreach (var role in roles)
        {
            if (await _roleManager.RoleExistsAsync(role.Name!))
                continue;

            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
                _logger.LogError("Błąd tworzenia roli {Role}: {Errors}", role.Name, FormatErrors(result));
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new SeedUser("F5BADE14-6CC8-42A2-9A44-9842DA2D9280", "admin@crm.pl", "Adam", "Administrator", "IT", "Admin@123!", UserRole.Administrator),
            new SeedUser("93A7FFDD-057F-4021-9C68-FE06951FFA65", "jan.kowalski@crm.pl", "Jan", "Kowalski", "Sales", "Manager@123!", UserRole.SalesManager),
            new SeedUser("3D4769E2-1C75-43E1-A5BB-1F71C68E9F57", "anna.nowak@crm.pl", "Anna", "Nowak", "Sales", "Sales@123!", UserRole.Salesperson),
            new SeedUser("0E136AB2-1A6A-4A16-938D-84DFB0F64BBA", "piotr.wisniewski@crm.pl", "Piotr", "Wiśniewski", "Sales", "Piotr123!", UserRole.Salesperson),
            new SeedUser("76B253D6-C16C-470A-943C-92F314A090F2", "maria.wojcik@crm.pl", "Maria", "Wójcik", "Support", "Support@123!", UserRole.SupportAgent),
            new SeedUser("E90A39C9-9CE2-400A-8A7B-8CF300D3B292", "tomasz.kaminski@crm.pl", "Tomasz", "Kamiński", "Management", "Readonly@123!", UserRole.ReadOnly)
        };

        foreach (var seedUser in users)
            await CreateUserAsync(seedUser);
    }

    private async Task CreateUserAsync(SeedUser seedUser)
    {
        if (await _userManager.FindByEmailAsync(seedUser.Email) is not null)
        {
            _logger.LogInformation("Użytkownik {Email} już istnieje — pomijam.", seedUser.Email);
            return;
        }

        var user = new CrmUser
        {
            Id = seedUser.Id,
            UserName = seedUser.Email,
            Email = seedUser.Email,
            FirstName = seedUser.FirstName,
            LastName = seedUser.LastName,
            FullName = $"{seedUser.FirstName} {seedUser.LastName}",
            Department = seedUser.Department,
            Status = SystemUserStatus.Active,
            EmailConfirmed = true,
            LockoutEnabled = true
        };

        user.Activate();

        // Let UserManager handle Password hashing and Normalization
        var createResult = await _userManager.CreateAsync(user, seedUser.Password);
        
        if (!createResult.Succeeded)
        {
            _logger.LogError("Błąd tworzenia użytkownika {Email}: {Errors}", user.Email, FormatErrors(createResult));
            return;
        }

        var roleResult = await _userManager.AddToRoleAsync(user, seedUser.Role.ToString());

        if (!roleResult.Succeeded)
        {
            _logger.LogError("Błąd przypisania roli {Role} dla {Email}: {Errors}", seedUser.Role, seedUser.Email, FormatErrors(roleResult));
            return;
        }

        _logger.LogInformation("Utworzono użytkownika {Email} z rolą {Role}.", seedUser.Email, seedUser.Role);
    }

    private static string FormatErrors(IdentityResult result) =>
        string.Join("; ", result.Errors.Select(e => e.Description));
}

internal record SeedUser(
    string Id,
    string Email,
    string FirstName,
    string LastName,
    string Department,
    string Password,
    UserRole Role
);