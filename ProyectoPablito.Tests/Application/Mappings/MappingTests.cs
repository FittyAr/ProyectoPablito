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
        var action = () => dto.Adapt<Trabajo>();

        // Assert
        action.Should().NotThrow<StackOverflowException>();
        var entity = action();
        entity.Descripcion.Should().Be("Test");
        entity.OrdenesTrabajo.Should().HaveCount(1);
    }
}
