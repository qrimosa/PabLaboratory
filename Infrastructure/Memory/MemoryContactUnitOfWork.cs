using AppCore.Interfaces;

namespace Infrastructure.Memory;

public class MemoryContactUnitOfWork(
    IPersonRepository persons,
    ICompanyRepository companies,
    IOrganizationRepository organizations) : IContactUnitOfWork
{
    public IPersonRepository Persons => persons;
    public ICompanyRepository Companies => companies;
    public IOrganizationRepository Organizations => organizations;

    public Task<int> SaveChangesAsync() => Task.FromResult(0);
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}