using AppCore.Dto;
using AppCore.Interfaces;
using FluentValidation;
using AppCore.Validators.Shared;

namespace AppCore.Validators;

public class CreatePersonDtoValidator : AbstractValidator<CreatePersonDto>
{
    private readonly ICompanyRepository _companyRepository;

    public CreatePersonDtoValidator(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100)
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Imię zawiera niedozwolone znaki.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(200)
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Nazwisko zawiera niedozwolone znaki.");

        RuleFor(x => x.Email)
            .NotEmpty().EmailAddress().MaximumLength(200);

        RuleFor(x => x.Phone)
            .Matches(@"^(\+\d{1,3})?[\s\-]?\d{3}[\s\-]?\d{3}[\s\-]?\d{3}$")
            .WithMessage("Nieprawidłowy format numeru telefonu.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today.AddYears(-18)).WithMessage("Osoba musi mieć co najmniej 18 lat.")
            .GreaterThan(DateTime.Today.AddYears(-120)).WithMessage("Nieprawidłowa data urodzenia.")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.EmployerId)
            .MustAsync(EmployerExistsAsync).WithMessage("Wskazana firma nie istnieje.")
            .When(x => x.EmployerId.HasValue);

        RuleFor(x => x.Address)
            .SetValidator(new AddressDtoValidator()!)
            .When(x => x.Address is not null);
    }

    private async Task<bool> EmployerExistsAsync(Guid? id, CancellationToken ct) => 
        await _companyRepository.FindByIdAsync(id ?? Guid.Empty) is not null;
}