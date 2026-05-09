using System;
using FluentAssertions;
using ProyectoPablito.Core.Entities;
using Xunit;

namespace ProyectoPablito.Tests.Core.Entities;

public class ClienteContactoTests
{
    [Fact]
    public void ClienteContacto_ShouldStoreProperties()
    {
        // Arrange
        var clienteId = Guid.NewGuid();
        var contacto = new ClienteContacto 
        { 
            ClienteId = clienteId,
            Etiqueta = "Ventas",
            Email = "juan@x.com"
        };

        // Assert
        contacto.ClienteId.Should().Be(clienteId);
        contacto.Etiqueta.Should().Be("Ventas");
        contacto.Email.Should().Be("juan@x.com");
    }
}
