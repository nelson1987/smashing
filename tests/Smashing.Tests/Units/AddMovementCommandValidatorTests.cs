using AutoFixture;
using AutoFixture.AutoMoq;
using FluentValidation;
using FluentValidation.TestHelper;
using Smashing.Core.Features.Movements;

namespace Smashing.Tests.Units;

public class AddMovementCommandValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly AddMovementCommand _renegotiationOrderRequested;
    private readonly IValidator<AddMovementCommand> _validator;

    public AddMovementCommandValidatorTests()
    {
        _renegotiationOrderRequested = _fixture
            .Build<AddMovementCommand>()
            .Create();
        _validator = _fixture.Create<AddMovementCommandValidator>();
    }

    [Fact]
    public void Given_a_valid_event_when_all_fields_are_valid_should_pass_validation()
    {
        _validator
            .TestValidate(_renegotiationOrderRequested)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Given_a_request_with_invalid_idempotencekey_should_fail_validation()
    {
        _validator
            .TestValidate(_renegotiationOrderRequested with { Valor = 0 })
            .ShouldHaveValidationErrorFor(x => x.Valor)
            .Only();
    }
}