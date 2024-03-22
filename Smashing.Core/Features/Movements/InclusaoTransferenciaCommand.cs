using FluentResults;
using FluentValidation;

namespace Smashing.Core.Features.Movements;

public class AddMovementCommandValidator : AbstractValidator<AddMovementCommand>
{
    public AddMovementCommandValidator()
    {
        RuleFor(x => x.Valor).NotEmpty();
    }
}

public interface IAddMovementCommandHandler
{
    Task<Result> Handle(AddMovementCommand command, CancellationToken cancellationToken);
}

public class AddMovementCommandHandler : IAddMovementCommandHandler
{
    private readonly IProducer _producer;
    private readonly IWriteRepository _writeRepository;

    public AddMovementCommandHandler(IWriteRepository writeRepository,
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
        BaseEntity aluno = command;
        await _writeRepository.Insert(aluno, cancellationToken);
        BaseEvent @event = aluno;
        await _producer.Send(@event, cancellationToken);
        return Result.Ok();
    }
}