using AppCore.Interfaces;
using Infrastructure.EntityFramework.Context;

namespace Infrastructure.EntityFramework.UnitOfWork;

public class EfContactsUnitOfWork(
    IPersonRepository personRepository,
    ICompanyRepository companyRepository,
    IOrganizationRepository organizationRepository,
    ContactsDbContext context
) : IContactUnitOfWork
{
    public IPersonRepository Persons => personRepository;
    public ICompanyRepository Companies => companyRepository;
    public IOrganizationRepository Organizations => organizationRepository;

    public async ValueTask DisposeAsync() => await context.DisposeAsync();

    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();

    public Task BeginTransactionAsync() => context.Database.BeginTransactionAsync();

    public Task CommitTransactionAsync() => context.Database.CommitTransactionAsync();

    public Task RollbackTransactionAsync() => context.Database.RollbackTransactionAsync();
}