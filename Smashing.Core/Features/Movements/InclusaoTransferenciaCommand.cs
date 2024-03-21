using FluentResults;
using FluentValidation;

namespace Smashing.Core.Features.Movements
{
    public record InclusaoTransferenciaCommand
    {
        public decimal Valor { get; init; }
    }
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
        private readonly IWriteRepository _writeRepository;
        private readonly IReadRepository _readRepository;
        private readonly IProducer _producer;

        public InclusaoTransferenciaCommandHandler(IWriteRepository writeRepository, IReadRepository readRepository, IProducer producer)
        {
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _producer = producer;
        }

        public async Task<Result> Handle(InclusaoTransferenciaCommand command, CancellationToken cancellationToken) 
        {
            return Result.Ok();
        }
    }
}
