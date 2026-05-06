using System;
using FluentAssertions;
using NSubstitute;
using ProyectoPablito.Application.Interfaces;
using ProyectoPablito.ViewModels;
using Xunit;

namespace ProyectoPablito.Tests.UI.ViewModels;

public class MainViewModelTests
{
    private readonly ILocalizationService _localizationService;

    public MainViewModelTests()
    {
        _localizationService = Substitute.For<ILocalizationService>();
    }

    [Fact]
    public void Constructor_ShouldSetGreetingFromLocalizationService()
    {
        // Arrange
        var serviceProvider = Substitute.For<IServiceProvider>();
        var localization = Substitute.For<ILocalizationService>();
        
        serviceProvider.GetService(typeof(DashboardViewModel)).Returns(new DashboardViewModel());
        _localizationService.GetString("General.AppName").Returns("Proyecto Pablito");

        // Act
        var vm = new MainViewModel(_localizationService, serviceProvider);

        // Assert
        vm.Greeting.Should().Be("Proyecto Pablito");
        _localizationService.Received(1).GetString("General.AppName");
    }
}
