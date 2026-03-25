using AppCore.Models;
using AppCore.Interfaces;
using AppCore.ValueObjects;
using Infrastructure.Memory;

public class MemoryGenericRepositoryTest
{
    private readonly IGenericRepositoryAsync<Person> _repo = new MemoryGenericRepository<Person>();

    [Fact]
    public async Task AddPersonTestAsync()
    {
        // Arrange
        var person = new Person 
        { 
            FirstName = "Adam", 
            LastName = "Kowalski",
            Address = new Address { Street="", City="", ZipCode= "", Country=Country.GB } 
        };

        // Act
        await _repo.AddAsync(person);
        var actual = await _repo.FindByIdAsync(person.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal("Adam", actual.FirstName);
        Assert.Equal(person.Id, actual?.Id);
    }

    [Fact]
    public async Task RemovePersonTestAsync()
    {
        // Arrange
        var person = new Person { FirstName = "To Delete" };
        await _repo.AddAsync(person);

        // Act
        await _repo.RemoveByIdAsync(person.Id);
        var actual = await _repo.FindByIdAsync(person.Id);

        // Assert
        Assert.Null(actual);
    }
}