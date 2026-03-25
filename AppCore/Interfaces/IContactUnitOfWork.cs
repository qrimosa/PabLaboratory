using AppCore.Interfaces;

namespace AppCore.Interfaces;

public interface IContactUnitOfWork : IAsyncDisposable
{
    IPersonRepository Persons { get; }
    ICompanyRepository Companies { get; }
    IOrganizationRepository Organizations { get; }
    Task<int> SaveChangesAsync();
}