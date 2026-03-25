using AppCore.Enums;
using AppCore.Interfaces;
using AppCore.Models;

namespace Infrastructure.Memory;

public class MemoryPersonRepository : MemoryGenericRepository<Person>, IPersonRepository
{
    public MemoryPersonRepository()
    {
        // Sample data for testing
        var id1 = Guid.NewGuid();
        _data.Add(id1, new Person { Id = id1, FirstName = "Adam", LastName = "Nowak", Gender = Gender.Male, Status = ContactStatus.Active });
        
        var id2 = Guid.NewGuid();
        _data.Add(id2, new Person { Id = id2, FirstName = "Ewa", LastName = "Kowalska", Gender = Gender.Female, Status = ContactStatus.Active });
    }

    public Task<IEnumerable<Person>> GetEmployeesByCompanyAsync(Guid companyId) 
        => Task.FromResult(_data.Values.Where(p => p.Employer?.Id == companyId));

    public Task<IEnumerable<Person>> GetMembersByOrganizationAsync(Guid organizationId)
        => throw new NotImplementedException("Implementation planned for home task.");
}

public class MemoryCompanyRepository : MemoryGenericRepository<Company>, ICompanyRepository
{
    public Task<IEnumerable<Company>> FindByNameAsync(string name)
        => Task.FromResult(_data.Values.Where(c => c.Name.Contains(name, StringComparison.OrdinalIgnoreCase)));

    public Task<Company?> FindByNipAsync(string nip)
        => Task.FromResult(_data.Values.FirstOrDefault(c => c.NIP == nip));
}

public class MemoryOrganizationRepository : MemoryGenericRepository<Organization>, IOrganizationRepository
{
    public Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type)
        => Task.FromResult(_data.Values.Where(o => o.Type == type));
}