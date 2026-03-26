using AppCore.Dto;
using FluentValidation;

namespace AppCore.Validators.Shared;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .Matches(@"^\d{2}-\d{3}$").WithMessage("Kod pocztowy musi być w formacie xx-xxx.");
        RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Type).IsInEnum();
    }
}