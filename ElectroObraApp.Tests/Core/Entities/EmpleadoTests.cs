using System;
using FluentAssertions;
using ElectroObraApp.Core.Entities;
using Xunit;

namespace ElectroObraApp.Tests.Core.Entities;

public class EmpleadoTests
{
    [Fact]
    public void NewEmpleado_ShouldHaveCollectionsInitialized()
    {
        // Act
        var empleado = new Empleado();

        // Assert
        empleado.AdelantosYPagos.Should().NotBeNull();
        empleado.Liquidaciones.Should().NotBeNull();
        empleado.Activo.Should().BeTrue();
    }
}

