using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Smashing.Core.Bases;
using Smashing.Core.Features.Movements;

namespace Smashing.Tests.Units;

public class MovementControllerUnitTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
    private readonly CancellationToken _token = CancellationToken.None;
    private readonly Mock<IReadRepository<Movement>> _readRepositoryMock;
    private readonly Mock<IValidator<AddMovementCommand>> _validator;
    private readonly Mock<IAddMovementCommandHandler> _resendHandler;
    private readonly List<Movement> _students;
    private readonly MovementController _sut;
    private readonly AddMovementCommand _command;

    public MovementControllerUnitTests()
    {
        _students = _fixture.Build<Movement>()
            .CreateMany(5)
            .ToList();

        _readRepositoryMock = _fixture.Freeze<Mock<IReadRepository<Movement>>>();
        _readRepositoryMock
            .Setup(x => x.GetAsync(_token))
            .ReturnsAsync(_students);
        _readRepositoryMock
            .Setup(x => x.GetAsync(_students[0].Id, _token))
            .ReturnsAsync(_students[0]);

        _command = _fixture.Build<AddMovementCommand>()
            .With(x =>x.Valor, 10.00M)
            .Create();

        _validator = _fixture.Freeze<Mock<IValidator<AddMovementCommand>>>();
        _validator
            .Setup(x => x.Validate(_command))
            .Returns(new ValidationResult());

        _resendHandler = _fixture.Freeze<Mock<IAddMovementCommandHandler>>();

        _sut = _fixture.Build<MovementController>()
            .OmitAutoProperties()
            .Create();
    }

    [Fact]
    public async Task Dado_Request_Valido_Metodo_Get_Retorno_200Ok()
    {
        // Act
        var result = await _sut.Get(_token);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(_students);
    }

    [Fact]
    public async Task Dado_Request_Valido_Metodo_GetById_Retorno_200Ok()
    {
        var expectedStudent = _students[0];
        // Act
        var result = await _sut.GetById(expectedStudent.Id, _token);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedStudent);
    }

    [Fact]
    public async Task Dado_Request_Valido_Metodo_Post_Retorno_200Ok()
    {
        // Act
        var result = await _sut.Post(_command, _token);

        // Assert
        result.Should().BeOfType<CreatedResult>();
        _validator.Verify(x => x.Validate(_command), Times.Once);
        _resendHandler.Verify(x => x.Handle(It.IsNotNull<AddMovementCommand>(), _token), Times.Once);
    }
}