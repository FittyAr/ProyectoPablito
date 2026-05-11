using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using ElectroObraApp.Infrastructure.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ElectroObraApp.Tests.Infrastructure.Services;

public class UserSettingsServiceTests
{
    [Fact]
    public void GetPageSize_ShouldReturnDefault_WhenNotSet()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
        var config = new ConfigurationBuilder().Build();
        var service = new UserSettingsService(config, NullLogger<UserSettingsService>.Instance, tempFile);

        // Act
        var result = service.GetPageSize();

        // Assert
        result.Should().Be(30);
        
        if (File.Exists(tempFile)) File.Delete(tempFile);
    }

    [Fact]
    public void GetPageSize_ShouldReturnFileValue()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
        Directory.CreateDirectory(Path.GetDirectoryName(tempFile)!);
        File.WriteAllText(tempFile, "{ \"Application\": { \"LastPageSize\": 50 } }");
        
        var config = new ConfigurationBuilder().Build();
        var service = new UserSettingsService(config, NullLogger<UserSettingsService>.Instance, tempFile);

        // Act
        var result = service.GetPageSize();

        // Assert
        result.Should().Be(50);
        
        if (File.Exists(tempFile)) File.Delete(tempFile);
    }
    
    [Fact]
    public void GetDefaultIncludeSaturday_ShouldReturnDefault_WhenNotSet()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
        var config = new ConfigurationBuilder().Build();
        var service = new UserSettingsService(config, NullLogger<UserSettingsService>.Instance, tempFile);

        // Act
        var result = service.GetDefaultIncludeSaturday();

        // Assert
        result.Should().BeFalse();
        
        if (File.Exists(tempFile)) File.Delete(tempFile);
    }

    [Fact]
    public void GetDefaultIncludeSaturday_ShouldReturnFileValue()
    {
        // Arrange
        var tempFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".json");
        Directory.CreateDirectory(Path.GetDirectoryName(tempFile)!);
        File.WriteAllText(tempFile, "{ \"Application\": { \"Settlement\": { \"IncludeSaturday\": true } } }");
        
        var config = new ConfigurationBuilder().Build();
        var service = new UserSettingsService(config, NullLogger<UserSettingsService>.Instance, tempFile);

        // Act
        var result = service.GetDefaultIncludeSaturday();

        // Assert
        result.Should().BeTrue();
        
        if (File.Exists(tempFile)) File.Delete(tempFile);
    }
}
