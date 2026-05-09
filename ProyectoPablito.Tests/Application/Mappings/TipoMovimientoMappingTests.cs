using FluentAssertions;
using Mapster;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Mappings;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Application.Mappings;

public class TipoMovimientoMappingTests
{
    public TipoMovimientoMappingTests()
    {
        MappingConfig.Configure();
    }

    [Fact]
    public void TipoMovimiento_To_Dto_ShouldMapFlags()
    {
        // Arrange
        var entity = new TipoMovimiento { Nombre = "Ingreso", EsIngreso = true };

        // Act
        var dto = entity.Adapt<TipoMovimientoDto>();

        // Assert
        dto.Nombre.Should().Be("Ingreso");
        dto.EsIngreso.Should().BeTrue();
    }
}
