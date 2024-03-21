using Microsoft.AspNetCore.Mvc;
using Smashing.Core;

namespace Smashing.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class StudentController : ControllerBase
{
    private readonly ILogger<StudentController> _logger;
    private readonly IProducer _producer;
    private readonly IReadRepository _readRepository;
    private readonly IWriteRepository _writeRepository;

    public StudentController(ILogger<StudentController> logger,
        IWriteRepository writeRepository,
        IReadRepository readRepository,
        IProducer producer)
    {
        _logger = logger;
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _producer = producer;
    }

    [HttpGet(Name = "GetAll")]
    public async Task<ActionResult<List<Student>>> Get(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Get");
        var listagem = await _readRepository.GetAll(cancellationToken);
        return Ok(listagem);
    }

    [HttpGet("{id:guid}", Name = "GetById")]
    public async Task<ActionResult<Student?>> GetById(Guid id, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GetById");
        var listagem = await _readRepository.GetAll(cancellationToken);
        return Ok(listagem.FirstOrDefault(x => x.Id == id));
    }

    [HttpPost(Name = "Post")]
    public async Task<ActionResult> Post(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Post");
        var estudante = new Student
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            Title = "Title",
            UserName = "Name"
        };
        await _writeRepository.Insert(estudante, cancellationToken);
        await _producer.Send(estudante, cancellationToken);
        return Created();
    }
}