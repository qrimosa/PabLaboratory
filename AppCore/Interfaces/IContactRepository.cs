using AppCore.Models;
using AppCore.Dto;
using AppCore.Enums;

namespace AppCore.Interfaces;

public interface IContactRepository : IGenericRepositoryAsync<Contact>
{
    Task<PagedResult<Contact>> SearchAsync(ContactSearchDto search);
    Task<IEnumerable<Contact>> FindByTagAsync(string tagName);
    Task AddNoteAsync(Guid contactId, Note note);
    Task<IEnumerable<Note>> GetNotesAsync(Guid contactId);
    Task AddTagAsync(Guid contactId, Tag tag);
    Task RemoveTagAsync(Guid contactId, string tagName);
}

public interface IPersonRepository : IGenericRepositoryAsync<Person>
{
    Task<IEnumerable<Person>> GetEmployeesByCompanyAsync(Guid companyId);
    Task<IEnumerable<Person>> GetMembersByOrganizationAsync(Guid organizationId);
    Task<Person?> FindByIdWithNotesAsync(Guid id);
}

public interface ICompanyRepository : IGenericRepositoryAsync<Company>
{
    Task<IEnumerable<Company>> FindByNameAsync(string name);
    Task<Company?> FindByNipAsync(string nip);
}

public interface IOrganizationRepository : IGenericRepositoryAsync<Organization>
{
    Task<IEnumerable<Organization>> FindByTypeAsync(OrganizationType type);
}