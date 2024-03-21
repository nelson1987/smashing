using AutoFixture;
using AutoFixture.AutoMoq;
using FluentValidation;
using FluentValidation.TestHelper;
using Smashing.Core;
using Smashing.Core.Features.Movements;

namespace Smashing.Tests.Units;

public class InclusaoTransferenciaCommandValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly InclusaoTransferenciaCommand _renegotiationOrderRequested;
    private readonly IValidator<InclusaoTransferenciaCommand> _validator;

    public InclusaoTransferenciaCommandValidatorTests()
    {
        _renegotiationOrderRequested = _fixture
            .Build<InclusaoTransferenciaCommand>()
            .Create();
        _validator = _fixture.Create<InclusaoTransferenciaCommandValidator>();
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