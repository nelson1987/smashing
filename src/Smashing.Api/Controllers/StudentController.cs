using Microsoft.AspNetCore.Mvc;
using Smashing.Core;

namespace Smashing.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController : ControllerBase
{
    private readonly ILogger<StudentController> _logger;
    private readonly IProducer _producer;
    private readonly IReadRepository<Movement> _readRepository;
    private readonly IWriteRepository<Movement> _writeRepository;

    public StudentController(ILogger<StudentController> logger,
        IWriteRepository<Movement> writeRepository,
        IReadRepository<Movement> readRepository,
        IProducer producer)
    {
        _logger = logger;
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _producer = producer;
    }

    [HttpGet(Name = "GetAll")]
    public async Task<ActionResult<List<Movement>>> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get");
        var listagem = await _readRepository.GetAsync(cancellationToken);
        return Ok(listagem);
    }

    [HttpGet("{id:guid}", Name = "GetById")]
    public async Task<ActionResult<Movement?>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetById");
        var listagem = await _readRepository.GetAsync(id, cancellationToken);
        return Ok(listagem);
    }

    [HttpPost(Name = "Post")]
    public async Task<ActionResult> Post(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Post");
        var estudante = new Movement
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Title = "Title",
            UserName = "Name"
        };
        await _writeRepository.CreateAsync(estudante, cancellationToken);
        await _producer.Send(estudante, cancellationToken);
        return Created();
    }
}