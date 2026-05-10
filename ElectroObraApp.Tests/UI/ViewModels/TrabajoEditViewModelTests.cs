using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ElectroObraApp.Application.DTOs;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.ViewModels;
using Xunit;

namespace ElectroObraApp.Tests.UI.ViewModels;

public class TrabajoEditViewModelTests
{
    private readonly ITrabajoService _trabajoService;
    private readonly IClienteService _clienteService;
    private readonly TrabajoEditViewModel _viewModel;

    public TrabajoEditViewModelTests()
    {
        _trabajoService = Substitute.For<ITrabajoService>();
        _clienteService = Substitute.For<IClienteService>();
        _viewModel = new TrabajoEditViewModel(_trabajoService, _clienteService);
    }

    [Fact]
    public async Task SaveCommand_ShouldCreate_WhenIdIsEmpty()
    {
        // Arrange
        _viewModel.Trabajo.Descripcion = "Nuevo";
        _trabajoService.CreateAsync(_viewModel.Trabajo).Returns(true);
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _trabajoService.Received(1).CreateAsync(Arg.Any<TrabajoDto>());
        closed.Should().BeTrue();
    }

    [Fact]
    public async Task SaveCommand_ShouldUpdate_WhenIdIsNotEmpty()
    {
        // Arrange
        _viewModel.Trabajo = new TrabajoDto { Id = Guid.NewGuid(), Descripcion = "Update" };
        _trabajoService.UpdateAsync(_viewModel.Trabajo).Returns(true);
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _trabajoService.Received(1).UpdateAsync(Arg.Any<TrabajoDto>());
        closed.Should().BeTrue();
    }

    [Fact]
    public void AddOrden_ShouldAddToList()
    {
        // Act
        _viewModel.AddOrdenCommand.Execute(null);

        // Assert
        _viewModel.Trabajo.OrdenesTrabajo.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveOrden_ShouldRemoveFromList()
    {
        // Arrange
        var orden = new OrdenTrabajoDto { Titulo = "Test" };
        _viewModel.Trabajo.OrdenesTrabajo.Add(orden);

        // Act
        _viewModel.RemoveOrdenCommand.Execute(orden);

        // Assert
        _viewModel.Trabajo.OrdenesTrabajo.Should().BeEmpty();
    }

    [Fact]
    public void AddItem_ShouldAddToList()
    {
        // Arrange
        var orden = new OrdenTrabajoDto { Titulo = "Test" };
        _viewModel.Trabajo.OrdenesTrabajo.Add(orden);

        // Act
        _viewModel.AddItemCommand.Execute(orden);

        // Assert
        orden.Items.Should().HaveCount(1);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveFromList()
    {
        // Arrange
        var orden = new OrdenTrabajoDto { Titulo = "Test" };
        var item = new OrdenTrabajoItemDto { Descripcion = "Item" };
        orden.Items.Add(item);
        _viewModel.Trabajo.OrdenesTrabajo.Add(orden);

        // Act
        _viewModel.RemoveItemCommand.Execute(item);

        // Assert
        orden.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task LoadData_ShouldPopulateClientes()
    {
        // Arrange
        var clientes = new List<ClienteDto> { new() { Nombre = "Cliente 1" } };
        _clienteService.GetAllAsync().Returns(clientes);

        // Act
        await _viewModel.LoadDataCommand.ExecuteAsync(null);

        // Assert
        _viewModel.Clientes.Should().HaveCount(1);
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

