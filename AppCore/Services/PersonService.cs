using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Models;
using AutoMapper;
using System.Linq;

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
        if (person == null) throw new Exception("Person not found");
        return mapper.Map<PersonDto>(person);
    }

    public async Task<PersonDto> AddPerson(CreatePersonDto personDto)
    {
        var person = mapper.Map<Person>(personDto);
        person.Id = Guid.NewGuid();
        person.CreatedAt = DateTime.UtcNow;
        
        await unitOfWork.Persons.AddAsync(person);
        await unitOfWork.SaveChangesAsync(); // Writes to contacts.db
        
        return mapper.Map<PersonDto>(person);
    }

    public async Task<PersonDto?> UpdatePerson(Guid id, UpdatePersonDto personDto)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);
        if (person == null) return null;

        // Apply changes from DTO to the Entity
        mapper.Map(personDto, person);
        person.UpdatedAt = DateTime.UtcNow;

        await unitOfWork.Persons.UpdateAsync(person);
        await unitOfWork.SaveChangesAsync();

        return mapper.Map<PersonDto>(person);
    }

    public async Task DeletePerson(Guid id)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);
        if (person != null)
        {
            await unitOfWork.Persons.RemoveByIdAsync(id);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<PersonDto?> FindById(Guid id)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);
        return person == null ? null : mapper.Map<PersonDto>(person);
    }

    public async Task<Note> AddNoteToPerson(Guid personId, CreateNoteDto noteDto)
    {
        // 1. Map DTO to Entity
        var note = mapper.Map<Note>(noteDto);
        note.Id = Guid.NewGuid();
        note.CreatedAt = DateTime.UtcNow;

        // 2. We use the Shadow Property name found in your database: "ContactId"
        // We access the Context directly from the UnitOfWork to set this value.
        // Note: If 'unitOfWork.Context' isn't public, you might need to add a method 
        // to your UoW or use the Person repository's internal context.
    
        var person = await unitOfWork.Persons.FindByIdAsync(personId);
        if (person == null) throw new Exception("Person not found");
        
        if (person.Notes == null) person.Notes = new List<Note>();
        person.Notes.Add(note);

        await unitOfWork.SaveChangesAsync();
        return note;
    }

    public async Task DeleteNoteFromPerson(Guid personId, Guid noteId)
    {
        var person = await unitOfWork.Persons.FindByIdWithNotesAsync(personId);
        if (person == null) return;

        var note = person.Notes.FirstOrDefault(n => n.Id == noteId);
        if (note != null)
        {
            person.Notes.Remove(note);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<PersonDto>> FindPeopleFromCompany(Guid companyId)
    {
        var people = await unitOfWork.Persons.GetEmployeesByCompanyAsync(companyId);
        return mapper.Map<List<PersonDto>>(people);
    }
}