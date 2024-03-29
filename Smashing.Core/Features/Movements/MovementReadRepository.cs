﻿using Smashing.Core.Bases;

namespace Smashing.Core.Features.Movements;

public class MovementReadRepository : ReadRepository<Movement>
{
    public MovementReadRepository(IReadContext context) : base(context, "Movements")
    {
    }
}