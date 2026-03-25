using AppCore.Interfaces;
using AppCore.Dto;

namespace Infrastructure.Memory;

public class MemoryPersonService(IContactUnitOfWork unitOfWork) : IPersonService
{
    public async Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size)
    {
        var result = await unitOfWork.Persons.FindPagedAsync(page, size);
        var dtos = result.Items.Select(p => (PersonDto)p).ToList();
        
        return new PagedResult<PersonDto>(dtos, result.TotalCount, page, size);
    }

    public async Task<PersonDto?> FindById(Guid id)
    {
        var person = await unitOfWork.Persons.FindByIdAsync(id);
        return person != null ? (PersonDto)person : null;
    }

    public async Task DeletePerson(Guid id) => await unitOfWork.Persons.RemoveByIdAsync(id);
}