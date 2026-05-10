using System;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using ElectroObraApp.Infrastructure.Services;
using Xunit;

namespace ElectroObraApp.Tests.Infrastructure.Services;

public class UserSettingsServiceTests
{
    [Fact]
    public void GetPageSize_ShouldReturnDefault_WhenNotSet()
    {
        // Arrange
        var config = new ConfigurationBuilder().Build();
        var service = new UserSettingsService(config);

        // Act
        var result = service.GetPageSize();

        // Assert
        result.Should().Be(30);
    }

    [Fact]
    public void GetPageSize_ShouldReturnConfigValue()
    {
        // Arrange
        var settings = new Dictionary<string, string?> { ["Application:LastPageSize"] = "50" };
        var config = new ConfigurationBuilder().AddInMemoryCollection(settings).Build();
        var service = new UserSettingsService(config);

        // Act
        var result = service.GetPageSize();

        // Assert
        result.Should().Be(50);
    }
}

