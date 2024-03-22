using FluentValidation;

namespace Smashing.Core.Features.Movements;

public class AddMovementCommandValidator : AbstractValidator<AddMovementCommand>
{
    public AddMovementCommandValidator()
    {
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}
