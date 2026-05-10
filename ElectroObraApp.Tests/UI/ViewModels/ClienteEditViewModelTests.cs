using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.ViewModels;
using Xunit;

namespace ElectroObraApp.Tests.UI.ViewModels;

public class ClienteEditViewModelTests
{
    private readonly IClienteService _clienteService;
    private readonly ClienteEditViewModel _viewModel;

    public ClienteEditViewModelTests()
    {
        _clienteService = Substitute.For<IClienteService>();
        _viewModel = new ClienteEditViewModel(_clienteService);
    }

    [Fact]
    public async Task SaveCommand_ShouldCreateClient_WhenIdIsEmpty()
    {
        // Arrange
        _viewModel.Cliente.Nombre = "Nuevo";
        _clienteService.CreateAsync(_viewModel.Cliente).Returns(true);
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _clienteService.Received(1).CreateAsync(Arg.Any<ClienteDto>());
        closed.Should().BeTrue();
    }

    [Fact]
    public void AddContact_ShouldAddToList()
    {
        // Act
        _viewModel.AddContactCommand.Execute(null);

        // Assert
        _viewModel.Cliente.Contactos.Should().HaveCount(1);
        _viewModel.Cliente.Contactos[0].Etiqueta.Should().Be("General");
    }

    [Fact]
    public void RemoveContact_ShouldRemoveFromList()
    {
        // Arrange
        var contacto = new ClienteContactoDto { Etiqueta = "Test" };
        _viewModel.Cliente.Contactos.Add(contacto);

        // Act
        _viewModel.RemoveContactCommand.Execute(contacto);

        // Assert
        _viewModel.Cliente.Contactos.Should().BeEmpty();
    }

    [Fact]
    public async Task SaveCommand_ShouldUpdateClient_WhenIdIsNotEmpty()
    {
        // Arrange
        _viewModel.Cliente = new ClienteDto { Id = Guid.NewGuid(), Nombre = "Update" };
        _clienteService.UpdateAsync(_viewModel.Cliente).Returns(true);
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _clienteService.Received(1).UpdateAsync(Arg.Any<ClienteDto>());
        closed.Should().BeTrue();
    }

    [Fact]
    public void CancelCommand_ShouldInvokeCloseRequestWithFalse()
    {
        // Arrange
        bool closedWithSuccess = true;
        _viewModel.CloseRequest += (s, success) => closedWithSuccess = success;

        // Act
        _viewModel.CancelCommand.Execute(null);

        // Assert
        closedWithSuccess.Should().BeFalse();
    }
}

