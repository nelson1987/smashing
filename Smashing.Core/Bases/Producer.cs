namespace Smashing.Core.Bases;

public interface IProducer
{
    Task Send(BaseEvent @event, CancellationToken cancellationToken);
}

public class Producer : IProducer
{
    private readonly IEventBus _eventBus;
    private readonly IWriteRepository<BaseEntity> _writeRepository;

    public Producer(IEventBus eventBus, IWriteRepository<BaseEntity> readContext)
    {
        _eventBus = eventBus;
        _writeRepository = readContext;
    }

    public async Task Send(BaseEvent @event, CancellationToken cancellationToken)
    {
        _eventBus.StudentEvents.Add(@event);
        await _writeRepository.CreateAsync(@event);
    }
}