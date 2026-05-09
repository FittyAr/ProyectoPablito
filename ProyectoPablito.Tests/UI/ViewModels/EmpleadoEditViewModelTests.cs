using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.ViewModels;
using Xunit;

namespace ProyectoPablito.Tests.UI.ViewModels;

public class EmpleadoEditViewModelTests
{
    private readonly IEmpleadoService _empleadoService;
    private readonly EmpleadoEditViewModel _viewModel;

    public EmpleadoEditViewModelTests()
    {
        _empleadoService = Substitute.For<IEmpleadoService>();
        _viewModel = new EmpleadoEditViewModel(_empleadoService);
    }

    [Fact]
    public async Task SaveCommand_ShouldCreateEmpleado_WhenIdIsEmpty()
    {
        // Arrange
        _viewModel.Empleado.Nombre = "Nuevo";
        _empleadoService.CreateAsync(_viewModel.Empleado).Returns(true);
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _empleadoService.Received(1).CreateAsync(Arg.Any<EmpleadoDto>());
        closed.Should().BeTrue();
    }

    [Fact]
    public void FechaIngresoOffset_ShouldUpdateEmpleadoFechaIngreso()
    {
        // Arrange
        var newDate = new DateTimeOffset(2026, 5, 1, 0, 0, 0, TimeSpan.Zero);

        // Act
        _viewModel.FechaIngresoOffset = newDate;

        // Assert
        _viewModel.Empleado.FechaIngreso.Should().Be(newDate.DateTime);
    }

    [Fact]
    public async Task SaveCommand_ShouldUpdateEmpleado_WhenIdIsNotEmpty()
    {
        // Arrange
        _viewModel.Empleado = new EmpleadoDto { Id = Guid.NewGuid(), Nombre = "Update" };
        _empleadoService.UpdateAsync(_viewModel.Empleado).Returns(true);
        bool closed = false;
        _viewModel.CloseRequest += (s, success) => closed = success;

        // Act
        await _viewModel.SaveCommand.ExecuteAsync(null);

        // Assert
        await _empleadoService.Received(1).UpdateAsync(Arg.Any<EmpleadoDto>());
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
