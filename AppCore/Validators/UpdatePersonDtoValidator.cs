using AppCore.Dto;
using AppCore.Interfaces;
using FluentValidation;
using AppCore.Validators.Shared;

namespace AppCore.Validators;

public class UpdatePersonDtoValidator : AbstractValidator<UpdatePersonDto>
{
    private readonly ICompanyRepository _companyRepository;

    public UpdatePersonDtoValidator(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100)
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Imię zawiera niedozwolone znaki.")
            .When(x => x.FirstName != null); // Only validate if provided

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(200)
            .Matches(@"^[\p{L}\s\-]+$").WithMessage("Nazwisko zawiera niedozwolone znaki.")
            .When(x => x.LastName != null);

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Nieprawidłowy format email.")
            .When(x => x.Email != null);

        RuleFor(x => x.Phone)
            .Matches(@"^(\+\d{1,3})?[\s\-]?\d{3}[\s\-]?\d{3}[\s\-]?\d{3}$")
            .WithMessage("Nieprawidłowy format numeru telefonu.")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.BirthDate)
            .LessThan(DateTime.Today.AddYears(-18)).WithMessage("Osoba musi mieć co najmniej 18 lat.")
            .When(x => x.BirthDate.HasValue);

        RuleFor(x => x.EmployerId)
            .MustAsync(EmployerExistsAsync).WithMessage("Wskazana firma nie istnieje.")
            .When(x => x.EmployerId.HasValue);

        RuleFor(x => x.Address)
            .SetValidator(new AddressDtoValidator()!)
            .When(x => x.Address is not null);
        
        RuleFor(x => x.Status)
            .NotNull().WithMessage("Status jest wymagany przy edycji.")
            .IsInEnum().WithMessage("Nieprawidłowy status.");
    }

    private async Task<bool> EmployerExistsAsync(Guid? id, CancellationToken ct) => 
        await _companyRepository.FindByIdAsync(id ?? Guid.Empty) is not null;
}