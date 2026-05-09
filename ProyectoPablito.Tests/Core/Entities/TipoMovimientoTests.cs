using FluentAssertions;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Core.Entities;

public class TipoMovimientoTests
{
    [Fact]
    public void NewTipoMovimiento_ShouldHaveId()
    {
        // Act
        var tipo = new TipoMovimiento();

        // Assert
        tipo.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void TipoMovimiento_ShouldStoreFlags()
    {
        // Arrange
        var tipo = new TipoMovimiento { EsIngreso = true, EsSistema = true };

        // Assert
        tipo.EsIngreso.Should().BeTrue();
        tipo.EsSistema.Should().BeTrue();
    }
}
