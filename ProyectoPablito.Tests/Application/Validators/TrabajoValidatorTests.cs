using System;
using FluentValidation.TestHelper;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Validators;
using Xunit;

namespace ProyectoPablito.Tests.Application.Validators;

public class TrabajoValidatorTests
{
    private readonly TrabajoValidator _validator;

    public TrabajoValidatorTests()
    {
        _validator = new TrabajoValidator();
    }

    [Fact]
    public void Should_HaveError_When_DescripcionIsEmpty()
    {
        var model = new TrabajoDto { Descripcion = "" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Descripcion);
    }

    [Fact]
    public void Should_HaveError_When_ClienteIdIsEmpty()
    {
        var model = new TrabajoDto { Descripcion = "Trabajo 1", ClienteId = Guid.Empty };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.ClienteId);
    }

    [Fact]
    public void Should_NotHaveError_When_ModelIsValid()
    {
        var model = new TrabajoDto { Descripcion = "Trabajo 1", ClienteId = Guid.NewGuid() };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
