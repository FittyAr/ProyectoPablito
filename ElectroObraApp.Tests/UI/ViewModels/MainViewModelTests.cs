using System;
using FluentAssertions;
using NSubstitute;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.ViewModels;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ElectroObraApp.Tests.UI.ViewModels;

public class MainViewModelTests
{
    private readonly ILocalizationService _localizationService;
    private readonly IConfiguration _configuration;

    public MainViewModelTests()
    {
        _localizationService = Substitute.For<ILocalizationService>();
        _configuration = Substitute.For<IConfiguration>();
    }

    [Fact]
    public void Constructor_ShouldSetGreetingFromLocalizationService()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var localization = Substitute.For<ILocalizationService>();
        var dashboardVm = new DashboardViewModel(Substitute.For<IMovimientoService>(), Substitute.For<IClienteService>(), Substitute.For<ITrabajoService>(), Substitute.For<IUserSettingsService>(), Substitute.For<IDollarService>());
        
        serviceProvider.GetService(typeof(DashboardViewModel)).Returns(dashboardVm);
        _configuration["Application:Name"].Returns("Proyecto Pablito");
        _localizationService.GetString("General.AppName").Returns("Proyecto Pablito");

        var seedService = Substitute.For<IDatabaseSeedService>();

        // Act
        var vm = new MainViewModel(_localizationService, serviceProvider, seedService, _configuration);

        // Assert
        vm.Greeting.Should().Be("Proyecto Pablito");
    }

    [Fact]
    public void NavigateToMovimientos_ShouldSetCurrentPage()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var seedService = Substitute.For<IDatabaseSeedService>();
        var movimientoService = Substitute.For<IMovimientoService>();
        var dashboardVm = new DashboardViewModel(Substitute.For<IMovimientoService>(), Substitute.For<IClienteService>(), Substitute.For<ITrabajoService>(), Substitute.For<IUserSettingsService>(), Substitute.For<IDollarService>());
        var movimientosVm = new MovimientosViewModel(movimientoService, Substitute.For<ITipoMovimientoService>(), Substitute.For<IExportService>(), Substitute.For<IUserSettingsService>(), serviceProvider);
        
        serviceProvider.GetService(typeof(DashboardViewModel)).Returns(dashboardVm);
        serviceProvider.GetService(typeof(MovimientosViewModel)).Returns(movimientosVm);
        _localizationService.GetString(Arg.Any<string>()).Returns("Test");

        var vm = new MainViewModel(_localizationService, serviceProvider, seedService, _configuration);

        // Act
        vm.NavigateToMovimientosCommand.Execute(null);

        // Assert
        vm.CurrentPage.Should().Be(movimientosVm);
    }

    [Fact]
    public void NavigateToTrabajos_ShouldSetCurrentPage()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var seedService = Substitute.For<IDatabaseSeedService>();
        var dashboardVm = new DashboardViewModel(Substitute.For<IMovimientoService>(), Substitute.For<IClienteService>(), Substitute.For<ITrabajoService>(), Substitute.For<IUserSettingsService>(), Substitute.For<IDollarService>());
        var trabajosVm = new TrabajosViewModel(Substitute.For<ITrabajoService>(), Substitute.For<IClienteService>(), Substitute.For<IUserSettingsService>(), serviceProvider);
        
        serviceProvider.GetService(typeof(DashboardViewModel)).Returns(dashboardVm);
        serviceProvider.GetService(typeof(TrabajosViewModel)).Returns(trabajosVm);
        _localizationService.GetString(Arg.Any<string>()).Returns("Test");

        var vm = new MainViewModel(_localizationService, serviceProvider, seedService, _configuration);

        // Act
        vm.NavigateToTrabajosCommand.Execute(null);

        // Assert
        vm.CurrentPage.Should().Be(trabajosVm);
    }

    [Fact]
    public void NavigateToClientes_ShouldSetCurrentPage()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var seedService = Substitute.For<IDatabaseSeedService>();
        var dashboardVm = new DashboardViewModel(Substitute.For<IMovimientoService>(), Substitute.For<IClienteService>(), Substitute.For<ITrabajoService>(), Substitute.For<IUserSettingsService>(), Substitute.For<IDollarService>());
        var clientesVm = new ClientesViewModel(Substitute.For<IClienteService>(), Substitute.For<IUserSettingsService>(), serviceProvider);
        serviceProvider.GetService(typeof(DashboardViewModel)).Returns(dashboardVm);
        serviceProvider.GetService(typeof(ClientesViewModel)).Returns(clientesVm);
        _localizationService.GetString(Arg.Any<string>()).Returns("Test");

        var vm = new MainViewModel(_localizationService, serviceProvider, seedService, _configuration);


        // Act
        vm.NavigateToClientesCommand.Execute(null);

        // Assert
        vm.CurrentPage.Should().Be(clientesVm);
    }

    [Fact]
    public void NavigateToEmpleados_ShouldSetCurrentPage()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var seedService = Substitute.For<IDatabaseSeedService>();
        var dashboardVm = new DashboardViewModel(Substitute.For<IMovimientoService>(), Substitute.For<IClienteService>(), Substitute.For<ITrabajoService>(), Substitute.For<IUserSettingsService>(), Substitute.For<IDollarService>());
        var empleadosVm = new EmpleadosViewModel(Substitute.For<IEmpleadoService>(), Substitute.For<IUserSettingsService>(), serviceProvider);
        
        serviceProvider.GetService(typeof(DashboardViewModel)).Returns(dashboardVm);
        serviceProvider.GetService(typeof(EmpleadosViewModel)).Returns(empleadosVm);
        _localizationService.GetString(Arg.Any<string>()).Returns("Test");

        var vm = new MainViewModel(_localizationService, serviceProvider, seedService, _configuration);

        // Act
        vm.NavigateToEmpleadosCommand.Execute(null);

        // Assert
        vm.CurrentPage.Should().Be(empleadosVm);
    }
}

