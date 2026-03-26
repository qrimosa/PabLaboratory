using AppCore.Dto;
using AppCore.Models;


namespace AppCore.Interfaces;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);
    Task<PersonDto?> FindById(Guid id);
    Task DeletePerson(Guid id);
    Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto);
    Task<PersonDto> GetPerson(Guid personId);
    Task DeleteNoteFromPerson(Guid personId, Guid noteId); 
}