using AppCore.Interfaces;
using AppCore.Models; // Ensure this matches your namespace for EntityBase
using AppCore.Dto;

namespace Infrastructure.Memory;

public class MemoryGenericRepository<T> : IGenericRepositoryAsync<T> where T : EntityBase
{
    protected readonly Dictionary<Guid, T> _data = new();

    public Task<T?> FindByIdAsync(Guid id)
    {
        var result = _data.GetValueOrDefault(id);
        return Task.FromResult(result);
    }

    public Task<IEnumerable<T>> FindAllAsync()
    {
        return Task.FromResult(_data.Values.AsEnumerable());
    }

    public Task<PagedResult<T>> FindPagedAsync(int page, int pageSize)
    {
        var items = _data.Values
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
            
        return Task.FromResult(new PagedResult<T>(items, _data.Count, page, pageSize));
    }

    public Task<T> AddAsync(T entity)
    {
        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        if (!_data.ContainsKey(entity.Id))
            throw new KeyNotFoundException($"Entity with ID {entity.Id} not found.");
            
        _data[entity.Id] = entity;
        return Task.FromResult(entity);
    }

    public Task RemoveByIdAsync(Guid id)
    {
        _data.Remove(id);
        return Task.CompletedTask;
    }
}