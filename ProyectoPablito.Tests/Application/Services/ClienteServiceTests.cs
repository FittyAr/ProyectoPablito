using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProyectoPablito.Application.DTOs;
using ProyectoPablito.Application.Services;
using ProyectoPablito.Core.Entities;
using ProyectoPablito.Core.Interfaces;
using Xunit;

namespace ProyectoPablito.Tests.Application.Services;

public class ClienteServiceTests
{
    private readonly IUnitOfWork _uow;
    private readonly IRepository<Cliente> _repo;
    private readonly ILogger<ClienteService> _logger;
    private readonly ClienteService _service;

    public ClienteServiceTests()
    {
        _uow = Substitute.For<IUnitOfWork>();
        _repo = Substitute.For<IRepository<Cliente>>();
        _logger = Substitute.For<ILogger<ClienteService>>();
        _uow.Repository<Cliente>().Returns(_repo);
        _service = new ClienteService(_uow, _logger);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnList()
    {
        // Arrange
        var list = new List<Cliente> { new() { Nombre = "Juan" } };
        _repo.GetAllAsync().Returns(list);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().Nombre.Should().Be("Juan");
    }
}
