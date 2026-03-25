using AppCore.Dto;
using AppCore.Models;

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