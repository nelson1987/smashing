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
    private readonly IInclusaoTransferenciaCommandHandler _handler;
    private readonly IValidator<InclusaoTransferenciaCommand> _validator;

    public MovementController(IValidator<InclusaoTransferenciaCommand> validator,
        IInclusaoTransferenciaCommandHandler handler)
    {
        _validator = validator;
        _handler = handler;
    }

    [HttpPost(Name = "Post")]
    public async Task<ActionResult> Post(InclusaoTransferenciaCommand command, CancellationToken cancellationToken)
    {
        //Validar transferencia válida
        var validationResult = _validator.Validate(command);
        if (validationResult.IsInvalid())
            return UnprocessableEntity(validationResult.ToModelState());

        var result = await _handler.Handle(command, cancellationToken);
        if (result.IsFailed)
            return BadRequest();

        return Created();
    }
}