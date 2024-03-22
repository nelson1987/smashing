using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Smashing.Core;

namespace Smashing.Tests.Units;

public class StudentControllerUnitTests
{
    private readonly StudentController _controller;
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly Mock<IReadRepository<Movement>> _readRepositoryMock;
    private readonly List<Movement> _students;
    private readonly CancellationToken _token = CancellationToken.None;

    public StudentControllerUnitTests()
    {
        _students = _fixture.Build<Movement>()
            .CreateMany(5)
            .ToList();

        _readRepositoryMock = _fixture.Freeze<Mock<IReadRepository<Movement>>>();
        _readRepositoryMock
            .Setup(x => x.GetAsync(_token))
            .ReturnsAsync(_students);

        _controller = _fixture.Build<StudentController>()
            .OmitAutoProperties()
            .Create();
    }

    [Fact]
    public async Task Dado_Request_Valido_Metodo_Get_Retorno_200Ok()
    {
        // Act
        var result = await _controller.Get(_token);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(_students);
    }

    [Fact]
    public async Task Dado_Request_Valido_Metodo_GetById_Retorno_200Ok()
    {
        var expectedStudent = _students[0];
        // Act
        var result = await _controller.GetById(expectedStudent.Id, _token);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedStudent);
    }

    [Fact]
    public async Task Dado_Request_Valido_Metodo_Post_Retorno_200Ok()
    {
        // Act
        var result = await _controller.Post(_token);

        // Assert
        result.Should().BeOfType<CreatedResult>();
    }
}