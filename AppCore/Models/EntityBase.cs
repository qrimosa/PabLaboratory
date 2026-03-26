using AppCore.Enums;
namespace AppCore.Models;

public abstract class EntityBase
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

public class Note : EntityBase
{
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
}
public class Tag : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
}

public abstract class Contact : EntityBase
{
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public Address Address { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ContactStatus Status { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public List<Note> Notes { get; set; } = new();

    public abstract string GetDisplayName();
}

public class Person : Contact
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public DateTime? BirthDate { get; set; }
    public Gender Gender { get; set; }
    public string? Position { get; set; }
    public Organization? Organization { get; set; }
    public Company? Employer { get; set; }

    public override string GetDisplayName() => $"{FirstName} {LastName}";
}

public class Company : Contact
{
    public string Name { get; set; } = string.Empty;
    public string? NIP { get; set; }
    public string? REGON { get; set; }
    public string? KRS { get; set; }
    public string? Industry { get; set; }
    public int? EmployeeCount { get; set; }
    public decimal? AnnualRevenue { get; set; }
    public string? Website { get; set; }
    public List<Person> Employees { get; set; } = new();
    public Person? PrimaryContact { get; set; }

    public override string GetDisplayName() => Name;
}

public class Organization : Contact
{
    public string Name { get; set; } = string.Empty;
    public OrganizationType Type { get; set; }
    public string? KRS { get; set; }
    public string? Website { get; set; }
    public string? Mission { get; set; }
    public List<Person> Members { get; set; } = new();
    public Person? PrimaryContact { get; set; }

    public override string GetDisplayName() => Name;
}