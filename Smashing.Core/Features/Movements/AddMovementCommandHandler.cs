using FluentResults;
using Smashing.Core.Bases;

namespace Smashing.Core.Features.Movements;

public interface IAddMovementCommandHandler
{
    Task<Result> Handle(AddMovementCommand command, CancellationToken cancellationToken);
}

public class AddMovementCommandHandler : IAddMovementCommandHandler
{
    private readonly IProducer _producer;
    private readonly IWriteRepository<Movement> _writeRepository;

    public AddMovementCommandHandler(IWriteRepository<Movement> writeRepository,
        IProducer producer)
    {
        _writeRepository = writeRepository;
        _producer = producer;
    }

    public async Task<Result> Handle(AddMovementCommand command, CancellationToken cancellationToken)
    {
        //Buscar as contas a serem debitadas e creditadas
        //Persistir os dados
        //Publicar Transferencia
        Movement aluno = command;
        await _writeRepository.CreateAsync(aluno, cancellationToken);
        //BaseEvent @event = aluno;
        //await _producer.Send(@event, cancellationToken);
        return Result.Ok();
    }
}