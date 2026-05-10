using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using ElectroObraApp.Application.Interfaces;
using ElectroObraApp.ViewModels;
using Xunit;

namespace ElectroObraApp.Tests.UI.ViewModels;

public class SeedViewModelTests
{
    private readonly IDatabaseSeedService _seedService;
    private readonly ILocalizationService _localizationService;
    private readonly SeedViewModel _viewModel;

    public SeedViewModelTests()
    {
        _seedService = Substitute.For<IDatabaseSeedService>();
        _localizationService = Substitute.For<ILocalizationService>();
        _localizationService.GetString(Arg.Any<string>()).Returns(x => (string)x[0]);

        _viewModel = new SeedViewModel(_seedService, _localizationService);
    }

    [Fact]
    public async Task SeedCommand_ShouldSetStatusToSuccess_WhenFinished()
    {
        // Act
        await _viewModel.SeedCommand.ExecuteAsync(null);

        // Assert
        await _seedService.Received(1).SeedAsync();
        _viewModel.StatusMessage.Should().Be("Seed.StatusSuccess");
        _viewModel.IsBusy.Should().BeFalse();
    }

    [Fact]
    public async Task SeedCommand_ShouldSetStatusToError_WhenExceptionOccurs()
    {
        // Arrange
        _seedService.When(x => x.SeedAsync()).Do(x => throw new Exception("Error fatal"));

        // Act
        await _viewModel.SeedCommand.ExecuteAsync(null);

        // Assert
        _viewModel.StatusMessage.Should().Contain("Seed.StatusError");
        _viewModel.StatusMessage.Should().Contain("Error fatal");
        _viewModel.IsBusy.Should().BeFalse();
    }
}

