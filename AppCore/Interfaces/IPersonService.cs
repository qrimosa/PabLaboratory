using AppCore.Dto;

namespace AppCore.Interfaces;

public interface IPersonService
{
    Task<PagedResult<PersonDto>> FindAllPeoplePaged(int page, int size);
    Task<PersonDto?> FindById(Guid id);
    Task DeletePerson(Guid id);
}