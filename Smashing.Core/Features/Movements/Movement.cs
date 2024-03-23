using Smashing.Core.Bases;

namespace Smashing.Core.Features.Movements;
public class MovementEvent : BaseEvent
{
    public decimal Valor { get; init; }
    public Status Status { get; init; }

    public static implicit operator MovementEvent(Movement v)
    {
        return new Movement
        {
            Valor = v.Valor,
            Status = v.Status
        };
    }

}
public class Movement : BaseEntity
{
    public decimal Valor { get; init; }
    public Status Status { get; init; }

    public static implicit operator Movement(AddMovementCommand v)
    {
        return new Movement
        {
            Valor = v.Valor,
            Status = Status.Pending
        };
    }
}
public enum Status
{
    Pending = 1,
    Rejected = 2,
    Created = 3
}