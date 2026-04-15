using AppCore.Models;
using AppCore.Enums;

namespace AppCore.Dto;

// --- Contact ---
public abstract record ContactBaseDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public AddressDto Address { get; init; } = null!;
    public ContactStatus Status { get; init; }
    public List<string> Tags { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}

public record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country,
    AddressType Type
)
{
    public AddressDto() : this(string.Empty, string.Empty, string.Empty, string.Empty, default) { }
}

// --- Person ---
public record PersonDto : ContactBaseDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string? Position { get; init; }
    public DateTime? BirthDate { get; init; }
    public Gender Gender { get; init; }
    public Guid? EmployerId { get; init; }
    public List<NoteDto> Notes { get; set; } = new();

    public static implicit operator PersonDto(Person p) => new()
    {
        Id = p.Id,
        FirstName = p.FirstName,
        LastName = p.LastName,
        Email = p.Email,
        Phone = p.Phone,
        Status = p.Status,
        CreatedAt = p.CreatedAt,
        Position = p.Position,
        BirthDate = p.BirthDate,
        Gender = p.Gender,
        EmployerId = p.Employer?.Id
    };
}

public record CreatePersonDto(
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    string? Position,
    DateTime? BirthDate,
    Gender Gender,
    Guid? EmployerId,
    AddressDto? Address
);

public record UpdatePersonDto(
    string? FirstName,
    string? LastName,
    string? Email,
    string? Phone,
    string? Position,
    DateTime? BirthDate,
    Gender? Gender,
    Guid? EmployerId,
    AddressDto? Address,
    ContactStatus? Status
);

// --- Contact Search ---
public record ContactSearchDto(
    string? Query,
    ContactStatus? Status,
    string? Tag,
    string? ContactType,
    int Page = 1,
    int PageSize = 20
);

// --- PagedResult ---
public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize
)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}

// --- Note ---
public class CreateNoteDto
{
    public string Content { get; set; } = string.Empty;
}

public class NoteDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; } 
}

//--- Login ---
public record LoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
	
public record AuthResponseDto
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
    public UserDto User { get; init; } = null!;
}
	
	
public record RefreshTokenDto(
    string AccessToken,
    string RefreshToken
);