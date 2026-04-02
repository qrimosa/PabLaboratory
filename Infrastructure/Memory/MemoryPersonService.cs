using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Models;
using AutoMapper;
using AppCore.Exceptions;

namespace Infrastructure.Memory;

public class MemoryPersonService(IContactUnitOfWork unitOfWork, IMapper mapper) : IPersonService
{
    public async Task<PersonDto?> FindById(Guid id)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id);
        return entity == null ? null : mapper.Map<PersonDto>(entity);
    }

    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var result = await unitOfWork.Persons.FindPagedAsync(page, size);
        var dtos = mapper.Map<List<PersonDto>>(result.Items);
        
        return new PagedResult<PersonDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<PersonDto> AddPerson(CreatePersonDto dto)
    {
        var entity = mapper.Map<Person>(dto);
        await unitOfWork.Persons.AddAsync(entity);
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<PersonDto>(entity);
    }

    public async Task<PersonDto?> UpdatePerson(Guid id, UpdatePersonDto dto)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id);
        if (entity == null) return null;

        mapper.Map(dto, entity);
        
        await unitOfWork.Persons.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
        
        return mapper.Map<PersonDto>(entity);
    }

    public async Task DeletePerson(Guid id)
    {
        await unitOfWork.Persons.RemoveByIdAsync(id);
        await unitOfWork.SaveChangesAsync();
    }

    public Task<IAsyncEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        async IAsyncEnumerable<PersonDto> Stream()
        {
            var people = await unitOfWork.Persons.GetEmployeesByCompanyAsync(companyId);
            foreach (var person in people)
            {
                yield return mapper.Map<PersonDto>(person);
            }
        }
        return Task.FromResult(Stream());
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(personId);
        if (entity == null) throw new ContactNotFoundException($"Person with id={personId} not found!");

        var note = mapper.Map<Note>(noteDto);
        note.Id = Guid.NewGuid();
        note.CreatedAt = DateTime.UtcNow;

        entity.Notes ??= new List<Note>();
        entity.Notes.Add(note);

        await unitOfWork.Persons.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    
        return note;
    }
    public async Task<PersonDto> GetPerson(Guid id)
    {
        var person = await FindById(id);
    
        if (person == null)
        {
            throw new AppCore.Exceptions.ContactNotFoundException($"Person with id={id} not found!");
        }
    
        return person;
    }

    public async Task DeleteNoteFromPerson(Guid personId, Guid noteId)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(personId);
        if (entity == null) throw new ContactNotFoundException($"Person with id={personId} not found!");

        var note = entity.Notes?.FirstOrDefault(n => n.Id == noteId);
        if (note != null)
        {
            entity.Notes!.Remove(note);
            await unitOfWork.Persons.UpdateAsync(entity);
            await unitOfWork.SaveChangesAsync();
        }
    }
}