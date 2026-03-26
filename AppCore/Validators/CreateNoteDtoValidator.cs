using AppCore.Dto;
using FluentValidation;

namespace AppCore.Validators;

public class CreateNoteDtoValidator : AbstractValidator<CreateNoteDto>
{
    public CreateNoteDtoValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Treść notatki nie może być pusta.")
            .MinimumLength(3).WithMessage("Notatka jest zbyt krótka (min. 3 znaki).")
            .MaximumLength(1000).WithMessage("Notatka nie może przekraczać 1000 znaków.");
    }
}