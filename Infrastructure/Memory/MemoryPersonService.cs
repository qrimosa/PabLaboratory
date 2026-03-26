using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Models;
using AutoMapper;
using AppCore.Exceptions;

namespace Infrastructure.Memory;

public class MemoryPersonService(IContactUnitOfWork unitOfWork, IMapper mapper) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var result = await unitOfWork.Persons.FindPagedAsync(page, size);
        
        var dtos = mapper.Map<List<PersonDto>>(result.Items);
        
        return new PagedResult<PersonDto>(dtos, result.TotalCount, result.Page, result.PageSize);
    }

    public async Task<PersonDto?> FindById(Guid id)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(id);
        return entity != null ? mapper.Map<PersonDto>(entity) : null;
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

    public async IAsyncEnumerable<PersonDto> FindPeopleFromCompany(Guid companyId)
    {
        var people = await unitOfWork.Persons.GetEmployeesByCompanyAsync(companyId);
        foreach (var person in people)
        {
            yield return mapper.Map<PersonDto>(person);
        }
    }
    
    public async Task<PersonDto> GetPerson(Guid personId)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(personId);
        if (entity == null) throw new ContactNotFoundException($"Person with id={personId} not found!");
        return mapper.Map<PersonDto>(entity);
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        var entity = await unitOfWork.Persons.FindByIdAsync(personId);
        if (entity == null) throw new ContactNotFoundException($"Person with id={personId} not found!");

        entity.Notes ??= new List<Note>();

        var note = new Note 
        { 
            Id = Guid.NewGuid(),
            Content = noteDto.Content,
            CreatedAt = DateTime.UtcNow 
        };

        entity.Notes.Add(note);
        await unitOfWork.Persons.UpdateAsync(entity);
        await unitOfWork.SaveChangesAsync();
    
        return note;
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