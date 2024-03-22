namespace Smashing.Core.Bases;

public interface IEventBus
{
    List<BaseEvent> StudentEvents { get; set; }
}

public class EventBus : IEventBus
{
    public EventBus()
    {
        StudentEvents = new List<BaseEvent>();
    }

    public List<BaseEvent> StudentEvents { get; set; }
}
