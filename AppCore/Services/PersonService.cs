using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Models;
using AutoMapper;

namespace AppCore.Services;

public class PersonService(IContactUnitOfWork unitOfWork, IMapper mapper) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var result = await unitOfWork.Persons.FindPagedAsync(page, size);
        var dtoItems = mapper.Map<List<PersonDto>>(result.Items);
        return new PagedResult<PersonDto>(dtoItems, result.TotalCount, page, size);
    }

    public async Task<PersonDto> GetPerson(Guid id)
    {
        var person = await unitOfWork.Persons.FindByIdWithNotesAsync(id);
        if (person == null) throw new Exception("Contact not found");
        return mapper.Map<PersonDto>(person);
    }

    public async Task<PersonDto> AddPerson(CreatePersonDto personDto)
    {
        var person = mapper.Map<Person>(personDto);
        person.Id = Guid.NewGuid();
        person.CreatedAt = DateTime.UtcNow;
        
        await unitOfWork.Persons.AddAsync(person);
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<PersonDto>(person);
    }

    
    public Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId) => throw new NotImplementedException();
    public Task<PersonDto?> FindById(Guid id) => throw new NotImplementedException();
    public Task DeletePerson(Guid id) => throw new NotImplementedException();
    public Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto) => throw new NotImplementedException();
    public Task DeleteNoteFromPerson(Guid personId, Guid noteId) => throw new NotImplementedException();
    public Task<PersonDto?> UpdatePerson(Guid id, UpdatePersonDto personDto) => throw new NotImplementedException();
}