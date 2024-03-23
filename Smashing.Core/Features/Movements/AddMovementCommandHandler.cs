using FluentResults;
using Smashing.Core.Bases;

namespace Smashing.Core.Features.Movements;

public interface IAddMovementCommandHandler
{
    Task<Result> Handle(AddMovementCommand command, CancellationToken cancellationToken);
}

public class AddMovementCommandHandler : IAddMovementCommandHandler
{
    private readonly IWriteRepository<Movement> _writeRepository;
    private readonly IProducer _producer;
    private readonly IHttpExternalServiceClient _httpExternalServiceClient;
    public AddMovementCommandHandler(IWriteRepository<Movement> writeRepository,
        IProducer producer,
        IHttpExternalServiceClient httpExternalServiceClient)
    {
        _writeRepository = writeRepository;
        _producer = producer;
        _httpExternalServiceClient = httpExternalServiceClient;
    }

    public async Task<Result> Handle(AddMovementCommand command, CancellationToken cancellationToken)
    {
        //Buscar as contas a serem debitadas e creditadas
        //Persistir os dados
        //Publicar Transferencia
        var autorizado = await _httpExternalServiceClient.GetTaskAsync(cancellationToken);
        if (!autorizado) return Result.Fail("Não Autorizado.");

        Movement aluno = command;
        await _writeRepository.CreateAsync(aluno, cancellationToken);

        MovementEvent @event = aluno;
        await _producer.Send(@event, cancellationToken);

        return Result.Ok();
    }
}
public interface IHttpExternalServiceClient { Task<bool> GetTaskAsync(CancellationToken cancellationToken); }
public class HttpExternalServiceClient : IHttpExternalServiceClient
{
    private readonly HttpClient Client;

    public HttpExternalServiceClient()
    {
        Client = new HttpClient();
    }

    public async Task<bool> GetTaskAsync(CancellationToken cancellationToken)
    {
        var result = await Client.GetAsync("https://www.google.com.br/", cancellationToken);
        return result.IsSuccessStatusCode;
    }
}