namespace Smashing.Core.Features.Movements;

public record AddMovementCommand
{
    public decimal Valor { get; init; }
}