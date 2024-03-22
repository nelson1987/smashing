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
    private readonly ILogger<MovementController> _logger;
    private readonly IValidator<AddMovementCommand> _validator;
    private readonly IReadRepository<Movement> _readRepository;
    private readonly IAddMovementCommandHandler _handler;

    public MovementController(ILogger<MovementController> logger,
        IValidator<AddMovementCommand> validator,
        IReadRepository<Movement> readRepository,
        IAddMovementCommandHandler handler)
    {
        _logger = logger;
        _validator = validator;
        _readRepository = readRepository;
        _handler = handler;
    }

    [HttpGet]
    public async Task<ActionResult<List<Movement>>> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get");
        var listagem = await _readRepository.GetAsync(cancellationToken);
        return Ok(listagem);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Movement?>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetById");
        var listagem = await _readRepository.GetAsync(id, cancellationToken);
        return Ok(listagem);
    }

    [HttpPost]
    public async Task<ActionResult> Post(AddMovementCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Post");
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