using Smashing.Core.Bases;

namespace Smashing.Core.Features.Movements;

public class MovementWriteRepository : WriteRepository<Movement>
{
    public MovementWriteRepository(IWriteContext context) : base(context)
    {
    }
}