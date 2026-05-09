using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.ViewModels;
using Xunit;

namespace ProyectoPablito.Tests.UI.ViewModels;

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
    public async Task SaveCommand_ShouldCreateTrabajo_WhenIdIsEmpty()
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
    public void AddOrden_ShouldAddToList()
    {
        // Act
        _viewModel.AddOrdenCommand.Execute(null);

        // Assert
        _viewModel.Trabajo.OrdenesTrabajo.Should().HaveCount(1);
        _viewModel.Trabajo.OrdenesTrabajo[0].Titulo.Should().Be("Nuevo Certificado");
    }

    [Fact]
    public void AddItem_ShouldAddToList()
    {
        // Arrange
        var orden = new OrdenTrabajoDto { Titulo = "Orden 1" };
        _viewModel.Trabajo.OrdenesTrabajo.Add(orden);

        // Act
        _viewModel.AddItemCommand.Execute(orden);

        // Assert
        orden.Items.Should().HaveCount(1);
        orden.Items[0].Descripcion.Should().Be("Nuevo Item");
    }
}
