using AppCore.Enums;
using AppCore.Models;

namespace AppCore.Interfaces;

public interface ICompanyRepository : IGenericRepositoryAsync<Company>
{
    Task<IEnumerable<Company>> FindByNameAsync(string name);
    Task<Company?> FindByNipAsync(string nip);
    Task<IEnumerable<Person>> GetEmployeesAsync(Guid companyId);
}