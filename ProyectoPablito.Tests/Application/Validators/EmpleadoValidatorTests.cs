using FluentValidation.TestHelper;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Validators;
using Xunit;

namespace ProyectoPablito.Tests.Application.Validators;

public class EmpleadoValidatorTests
{
    private readonly EmpleadoValidator _validator;

    public EmpleadoValidatorTests()
    {
        _validator = new EmpleadoValidator();
    }

    [Fact]
    public void Should_HaveError_When_NombreIsEmpty()
    {
        var model = new EmpleadoDto { Nombre = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Nombre);
    }

    [Fact]
    public void Should_HaveError_When_DniIsTooShort()
    {
        var model = new EmpleadoDto { Nombre = "Pablo", Dni = "12345" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Dni);
    }

    [Fact]
    public void Should_HaveError_When_TarifaIsNegative()
    {
        var model = new EmpleadoDto { Nombre = "Pablo", Dni = "12345678", TarifaDiaria = -100 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.TarifaDiaria);
    }

    [Fact]
    public void Should_NotHaveError_When_ModelIsValid()
    {
        var model = new EmpleadoDto { Nombre = "Pablo", Dni = "12345678", TarifaDiaria = 5000 };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
