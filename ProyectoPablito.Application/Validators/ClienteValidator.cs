using FluentValidation;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Validators;

public class ClienteValidator : AbstractValidator<ClienteDto>
{
    public ClienteValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.Email)
            .EmailAddress().When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("El formato del email no es válido.");

        RuleFor(x => x.Cuit)
            .Matches(@"^\d{2}-\d{8}-\d{1}$").When(x => !string.IsNullOrEmpty(x.Cuit))
            .WithMessage("El formato del CUIT debe ser XX-XXXXXXXX-X.");
    }
}
