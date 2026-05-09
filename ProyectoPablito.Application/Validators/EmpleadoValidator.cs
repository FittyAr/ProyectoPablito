using FluentValidation;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Validators;

public class EmpleadoValidator : AbstractValidator<EmpleadoDto>
{
    public EmpleadoValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede exceder los 100 caracteres.");

        RuleFor(x => x.Dni)
            .NotEmpty().WithMessage("El DNI es obligatorio.")
            .Length(7, 9).WithMessage("El DNI debe tener entre 7 y 9 dígitos.");

        RuleFor(x => x.TarifaDiaria)
            .GreaterThanOrEqualTo(0).WithMessage("La tarifa diaria no puede ser negativa.");
    }
}
