using AppCore.Dto;
using AppCore.Interfaces;
using AppCore.Models;
using AutoMapper;

namespace Infrastructure.Memory;

public class MemoryPersonService(IContactUnitOfWork unitOfWork, IMapper mapper) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        // 1. Get paged entities from repository
        var result = await unitOfWork.Persons.FindPagedAsync(page, size);
        
        // 2. Map the list of items
        var dtos = mapper.Map<List<PersonDto>>(result.Items);
        
        // 3. Wrap in a new PagedResult of DTOs
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

        // Map DTO values onto the existing entity
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

    // Implementing this from your interface requirements
    public async IAsyncEnumerable<PersonDto> FindPeopleFromCompany(Guid companyId)
    {
        var people = await unitOfWork.Persons.GetEmployeesByCompanyAsync(companyId);
        foreach (var person in people)
        {
            yield return mapper.Map<PersonDto>(person);
        }
    }
}