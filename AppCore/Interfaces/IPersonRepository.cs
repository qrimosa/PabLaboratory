using AppCore.Models;

namespace AppCore.Interfaces;

public interface IPersonRepository : IGenericRepositoryAsync<Person>
{
    Task<IEnumerable<Person>> GetEmployeesByCompanyAsync(Guid companyId);
    Task<IEnumerable<Person>> GetMembersByOrganizationAsync(Guid organizationId);
}