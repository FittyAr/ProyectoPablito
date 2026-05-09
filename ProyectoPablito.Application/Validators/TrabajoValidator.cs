using FluentValidation;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Validators;

public class TrabajoValidator : AbstractValidator<TrabajoDto>
{
    public TrabajoValidator()
    {
        RuleFor(x => x.Descripcion)
            .NotEmpty().WithMessage("La descripción es obligatoria.")
            .MaximumLength(200).WithMessage("La descripción no puede exceder los 200 caracteres.");

        RuleFor(x => x.ClienteId)
            .NotEmpty().WithMessage("Debe seleccionar un cliente.");
    }
}
