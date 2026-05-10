using FluentValidation;
using ElectroObraApp.Application.DTOs;

namespace ElectroObraApp.Application.Validators;

public class MovimientoValidator : AbstractValidator<MovimientoDto>
{
    public MovimientoValidator()
    {
        RuleFor(x => x.Concepto)
            .NotEmpty().WithMessage("El concepto no puede estar vacío.")
            .MaximumLength(200).WithMessage("El concepto no puede exceder los 200 caracteres.");

        RuleFor(x => x.Monto)
            .GreaterThan(0).WithMessage("El monto debe ser mayor a cero.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0).WithMessage("La cantidad debe ser mayor a cero.");

        RuleFor(x => x.CategoriaId)
            .NotEmpty().WithMessage("Debe seleccionar una categoría.");

        RuleFor(x => x.TipoMovimientoId)
            .NotEmpty().WithMessage("Debe seleccionar un tipo de movimiento.");
    }
}

