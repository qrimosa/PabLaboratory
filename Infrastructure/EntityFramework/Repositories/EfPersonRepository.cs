using AppCore.Enums;
using AppCore.Interfaces;
using AppCore.Models;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfPersonRepository(ContactsDbContext context) 
    : EfGenericRepository<Person>(context.People), IPersonRepository
{
    public async Task<IEnumerable<Person>> GetEmployeesByCompanyAsync(Guid companyId)
    {
        return await context.People
            .Where(p => p.Employer != null && p.Employer.Id == companyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetMembersByOrganizationAsync(Guid organizationId)
    {
        return await context.People
            .Where(p => p.Organization != null && p.Organization.Id == organizationId)
            .ToListAsync();
    }

    public async Task<Person?> FindByIdWithNotesAsync(Guid id)
    {
        return await context.People
            .Include(p => p.Notes)
            .Include(p => p.Address)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}

public class EfOrganizationRepository(ContactsDbContext context) 
    : EfGenericRepository<Organization>(context.Organizations), IOrganizationRepository
{
    public async Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type)
    {
        return await context.Organizations
            .Where(o => o.Type == type)
            .ToListAsync();
    }
}
