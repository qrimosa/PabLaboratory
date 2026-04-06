using AppCore.Interfaces;
using AppCore.Models;
using Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework.Repositories;

public class EfPersonRepository(ContactsDbContext context) 
    : EfGenericRepository<Person>(context.People), IPersonRepository
{
    public async Task<IEnumerable<Person>> FindPeopleFromCompany(Guid companyId)
    {
        return await context.People
            .Where(p => p.Employer != null && p.Employer.Id == companyId)
            .ToListAsync();
    }

    // This ensures that when we get a person, we also load their Notes from the DB
    public async Task<Person?> FindByIdWithNotesAsync(Guid id)
    {
        return await context.People
            .Include(p => p.Notes)
            .Include(p => p.Address)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
public class EfOrganizationRepository(ContactsDbContext context) 
    : EfGenericRepository<Organization>(context.Organizations), IOrganizationRepository { }