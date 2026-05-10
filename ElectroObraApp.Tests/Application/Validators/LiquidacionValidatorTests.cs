using System;
using FluentValidation.TestHelper;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Validators;
using Xunit;

namespace ElectroObraApp.Tests.Application.Validators;

public class LiquidacionValidatorTests
{
    private readonly LiquidacionValidator _validator;

    public LiquidacionValidatorTests()
    {
        _validator = new LiquidacionValidator();
    }

    [Fact]
    public void Should_HaveError_When_FechaInicioIsAfterFechaFin()
    {
        var model = new LiquidacionDto 
        { 
            FechaInicio = DateTime.Now.AddDays(1), 
            FechaFin = DateTime.Now 
        };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.FechaInicio);
    }

    [Fact]
    public void Should_HaveError_When_DiasTrabajadosIsZero()
    {
        var model = new LiquidacionDto { DiasTrabajados = 0 };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.DiasTrabajados);
    }

    [Fact]
    public void Should_NotHaveError_When_ModelIsValid()
    {
        var model = new LiquidacionDto 
        { 
            EmpleadoId = Guid.NewGuid(),
            FechaInicio = DateTime.Now.AddDays(-30),
            FechaFin = DateTime.Now,
            DiasTrabajados = 20,
            TarifaAplicada = 1000
        };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}

