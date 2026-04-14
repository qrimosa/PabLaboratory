using AppCore.Dto;
using AppCore.Models;

namespace AppCore.Interfaces;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);
    Task<IEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId);
    Task<PersonDto> AddPerson(CreatePersonDto personDto);
    Task<PersonDto?> UpdatePerson(Guid id, UpdatePersonDto personDto);
    Task<PersonDto?> FindById(Guid id);
    
    Task<PersonDto> GetPerson(Guid id); 
    
    Task DeletePerson(Guid id);
    Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto);
    Task DeleteNoteFromPerson(Guid personId, Guid noteId);
}