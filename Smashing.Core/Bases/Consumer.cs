namespace Smashing.Core.Bases;

public interface IConsumer
{
    Task<BaseEvent> Consume(CancellationToken cancellationToken);
}

public class Consumer : IConsumer
{
    private readonly IEventBus _eventBus;

    public Consumer(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<BaseEvent> Consume(CancellationToken cancellationToken)
    {
        return await Task.FromResult(_eventBus.StudentEvents.Last());
    }
}