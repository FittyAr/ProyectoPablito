using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.ViewModels;
using Xunit;

namespace ProyectoPablito.Tests.UI.ViewModels;

public class TrabajosViewModelTests
{
    private readonly ITrabajoService _trabajoService;
    private readonly IClienteService _clienteService;
    private readonly IUserSettingsService _settingsService;
    private readonly IServiceProvider _serviceProvider;
    private readonly TrabajosViewModel _viewModel;

    public TrabajosViewModelTests()
    {
        _trabajoService = Substitute.For<ITrabajoService>();
        _clienteService = Substitute.For<IClienteService>();
        _settingsService = Substitute.For<IUserSettingsService>();
        _serviceProvider = Substitute.For<IServiceProvider>();

        _settingsService.GetPageSize().Returns(10);
        _clienteService.GetAllAsync().Returns(new List<ClienteDto>());
        _trabajoService.GetAllAsync().Returns(new List<TrabajoDto>());

        _viewModel = new TrabajosViewModel(_trabajoService, _clienteService, _settingsService, _serviceProvider);
    }

    [Fact]
    public async Task LoadTrabajosAsync_ShouldPopulateTrabajos()
    {
        // Arrange
        var list = new List<TrabajoDto> { new() { Descripcion = "Test" } };
        _trabajoService.GetAllAsync().Returns(list);

        // Act
        await _viewModel.LoadTrabajosAsync();

        // Assert
        _viewModel.Trabajos.Should().HaveCount(1);
        _viewModel.Trabajos[0].Descripcion.Should().Be("Test");
    }

    [Fact]
    public async Task AddAsync_ShouldSetIsEditingAndEditViewModel()
    {
        // Arrange
        var editVm = new TrabajoEditViewModel(_trabajoService, _clienteService);
        _serviceProvider.GetService(typeof(TrabajoEditViewModel)).Returns(editVm);

        // Act
        await _viewModel.AddCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsEditing.Should().BeTrue();
        _viewModel.EditViewModel.Should().NotBeNull();
    }
}
