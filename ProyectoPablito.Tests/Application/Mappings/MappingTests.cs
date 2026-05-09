using FluentAssertions;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Mappings;
using ProyectoPablito.Core.Entities;
using Mapster;
using Xunit;
using System;

namespace ProyectoPablito.Tests.Application.Mappings;

public class MappingTests
{
    public MappingTests()
    {
        MappingConfig.Configure();
    }

    [Fact]
    public void TrabajoDto_To_Entity_ShouldNotStackOverflow()
    {
        // Arrange
        var dto = new TrabajoDto
        {
            Id = Guid.NewGuid(),
            Descripcion = "Test",
            OrdenesTrabajo = new System.Collections.ObjectModel.ObservableCollection<OrdenTrabajoDto>
            {
                new() 
                { 
                    Id = Guid.NewGuid(),
                    Titulo = "Cert 1",
                    Items = new System.Collections.ObjectModel.ObservableCollection<OrdenTrabajoItemDto>
                    {
                        new() { Id = Guid.NewGuid(), Descripcion = "Item 1" }
                    }
                }
            }
        };

        // Act
        var entity = dto.Adapt<Trabajo>();

        // Assert
        entity.Descripcion.Should().Be("Test");
        entity.OrdenesTrabajo.Should().HaveCount(1);
    }

    [Fact]
    public void Movimiento_To_Dto_ShouldMapNavProperties()
    {
        // Arrange
        var entity = new Movimiento
        {
            Concepto = "Sueldo",
            TipoMovimiento = new TipoMovimiento { Nombre = "Ingreso", EsIngreso = true },
            Categoria = new Categoria { Nombre = "Varios" }
        };

        // Act
        var dto = entity.Adapt<MovimientoDto>();

        // Assert
        dto.Concepto.Should().Be("Sueldo");
        dto.TipoMovimientoNombre.Should().Be("Ingreso");
        dto.CategoriaNombre.Should().Be("Varios");
        dto.EsIngreso.Should().BeTrue();
    }

    [Fact]
    public void ClienteDto_To_Entity_ShouldMapContactos()
    {
        // Arrange
        var dto = new ClienteDto
        {
            Nombre = "Cliente 1",
            Contactos = new System.Collections.ObjectModel.ObservableCollection<ClienteContactoDto>
            {
                new() { Etiqueta = "Email", Email = "test@test.com" }
            }
        };

        // Act
        var entity = dto.Adapt<Cliente>();

        // Assert
        entity.Nombre.Should().Be("Cliente 1");
        entity.Contactos.Should().HaveCount(1);
        entity.Contactos.First().Email.Should().Be("test@test.com");
    }
}
