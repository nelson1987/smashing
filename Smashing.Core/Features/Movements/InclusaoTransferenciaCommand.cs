using FluentResults;
using FluentValidation;

namespace Smashing.Core.Features.Movements;

public class InclusaoTransferenciaCommandValidator : AbstractValidator<InclusaoTransferenciaCommand>
{
    public InclusaoTransferenciaCommandValidator()
    {
        RuleFor(x => x.Valor).NotEmpty();
    }
}

public interface IInclusaoTransferenciaCommandHandler
{
    Task<Result> Handle(InclusaoTransferenciaCommand command, CancellationToken cancellationToken);
}

public class InclusaoTransferenciaCommandHandler : IInclusaoTransferenciaCommandHandler
{
    private readonly IProducer _producer;
    private readonly IReadRepository _readRepository;
    private readonly IWriteRepository _writeRepository;

    public InclusaoTransferenciaCommandHandler(IWriteRepository writeRepository, IReadRepository readRepository,
        IProducer producer)
    {
        _writeRepository = writeRepository;
        _readRepository = readRepository;
        _producer = producer;
    }

    public async Task<Result> Handle(InclusaoTransferenciaCommand command, CancellationToken cancellationToken)
    {
        Student aluno = command;
        await _writeRepository.Insert(aluno, cancellationToken);
        StudentEvent @event = aluno;
        await _producer.Send(@event, cancellationToken);
        return Result.Ok();
    }
}