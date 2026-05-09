using FluentValidation;
using ProyectoPablito.Application.DTOs;

namespace ProyectoPablito.Application.Validators;

public class LiquidacionValidator : AbstractValidator<LiquidacionDto>
{
    public LiquidacionValidator()
    {
        RuleFor(x => x.EmpleadoId)
            .NotEmpty().WithMessage("Debe seleccionar un empleado.");

        RuleFor(x => x.FechaInicio)
            .LessThanOrEqualTo(x => x.FechaFin).WithMessage("La fecha de inicio no puede ser posterior a la de fin.");

        RuleFor(x => x.DiasTrabajados)
            .GreaterThan(0).WithMessage("Los días trabajados deben ser mayores a cero.");

        RuleFor(x => x.TarifaAplicada)
            .GreaterThan(0).WithMessage("La tarifa aplicada debe ser mayor a cero.");
    }
}
