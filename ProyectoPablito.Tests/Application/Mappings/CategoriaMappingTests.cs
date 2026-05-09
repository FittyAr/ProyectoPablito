using FluentAssertions;
using Mapster;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Mappings;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Application.Mappings;

public class CategoriaMappingTests
{
    public CategoriaMappingTests()
    {
        MappingConfig.Configure();
    }

    [Fact]
    public void Categoria_To_Dto_ShouldMapCorrectly()
    {
        // Arrange
        var entity = new Categoria { Nombre = "Test", Descripcion = "Desc" };

        // Act
        var dto = entity.Adapt<CategoriaDto>();

        // Assert
        dto.Nombre.Should().Be("Test");
        dto.Descripcion.Should().Be("Desc");
    }

    [Fact]
    public void Dto_To_Categoria_ShouldMapCorrectly()
    {
        // Arrange
        var dto = new CategoriaDto { Nombre = "Test DTO" };

        // Act
        var entity = dto.Adapt<Categoria>();

        // Assert
        entity.Nombre.Should().Be("Test DTO");
    }
}
