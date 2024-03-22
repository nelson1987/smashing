using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Smashing.Core;
using Smashing.Core.Extensions;
using Smashing.Core.Features.Movements;

namespace Smashing.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MovementController : ControllerBase
{
    private readonly IAddMovementCommandHandler _handler;
    private readonly IValidator<AddMovementCommand> _validator;

    public MovementController(IValidator<AddMovementCommand> validator,
        IAddMovementCommandHandler handler)
    {
        _validator = validator;
        _handler = handler;
    }

    [HttpPost]
    public async Task<ActionResult> Post(AddMovementCommand command, CancellationToken cancellationToken)
    {
        //Validar transferencia válida
        var validationResult = _validator.Validate(command);
        //if (validationResult.IsInvalid())
            //return UnprocessableEntity(validationResult.ToModelState());
            if (validationResult.IsValid)
            {
                var result = await _handler.Handle(command, cancellationToken);
                if (result.IsFailed)
                    return BadRequest();
            }
            return Created();
    }
}