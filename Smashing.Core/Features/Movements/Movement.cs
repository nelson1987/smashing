using Smashing.Core.Bases;

namespace Smashing.Core.Features.Movements;

public class Movement : BaseEntity
{
    public decimal Valor { get; init; }

    public static implicit operator Movement(AddMovementCommand v)
    {
        return new Movement
        {
            Valor = v.Valor
        };
    }
}