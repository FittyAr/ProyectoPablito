using FluentValidation.TestHelper;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Validators;
using Xunit;

namespace ProyectoPablito.Tests.Application.Validators;

public class ClienteValidatorTests
{
    private readonly ClienteValidator _validator;

    public ClienteValidatorTests()
    {
        _validator = new ClienteValidator();
    }

    [Fact]
    public void Should_HaveError_When_EmailIsInvalid()
    {
        var model = new ClienteDto { Nombre = "Test", Email = "invalid-email" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Should_HaveError_When_CuitIsInvalid()
    {
        var model = new ClienteDto { Nombre = "Test", Cuit = "12345678" };
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Cuit);
    }

    [Fact]
    public void Should_NotHaveError_When_CuitIsValid()
    {
        var model = new ClienteDto { Nombre = "Test", Cuit = "20-30405060-7" };
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveValidationErrorFor(x => x.Cuit);
    }
}
