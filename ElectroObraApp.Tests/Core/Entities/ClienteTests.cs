using FluentAssertions;
using ElectroObraApp.Core.Entities;
using Xunit;

namespace ElectroObraApp.Tests.Core.Entities;

public class ClienteTests
{
    [Fact]
    public void NewCliente_ShouldInitializeCollections()
    {
        // Act
        var cliente = new Cliente();

        // Assert
        cliente.Movimientos.Should().NotBeNull();
        cliente.Contactos.Should().NotBeNull();
    }

    [Fact]
    public void Cliente_ShouldStoreProperties()
    {
        // Arrange
        var cliente = new Cliente 
        { 
            Nombre = "Empresa X", 
            Cuit = "20-12345678-9",
            Email = "info@x.com"
        };

        // Assert
        cliente.Nombre.Should().Be("Empresa X");
        cliente.Cuit.Should().Be("20-12345678-9");
        cliente.Email.Should().Be("info@x.com");
    }
}

