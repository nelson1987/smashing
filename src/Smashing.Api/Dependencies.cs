namespace Smashing.Api;

public static class Dependencies
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        services.AddSingleton<IWriteContext, WriteContext>()
        .AddSingleton<IReadContext, ReadContext>()
        .AddSingleton<IEventBus, EventBus>()
        .AddScoped<IWriteRepository, WriteRepository>()
        .AddScoped<IReadRepository, ReadRepository>()
        .AddScoped<IProducer, Producer>()
        .AddScoped<IConsumer, Consumer>();
        return services;

    }
}

public interface IWriteContext
{
    List<Student> Students { get; set; }
}
public class WriteContext : IWriteContext
{
    public List<Student> Students { get; set; }
    public WriteContext()
    {
        Students = new List<Student>();
    }
}
public interface IReadContext
{
    List<Student> Students { get; set; }
}
public class ReadContext : IReadContext
{
    public List<Student> Students { get; set; }
    public ReadContext()
    {
        Students = new List<Student>();
    }
}
public interface IEventBus
{
    List<StudentEvent> StudentEvents { get; set; }
}
public class EventBus : IEventBus
{
    public List<StudentEvent> StudentEvents { get; set; }
    public EventBus()
    {
        StudentEvents = new List<StudentEvent>();
    }
}

public interface IWriteRepository
{
    Task Insert(Student student, CancellationToken cancellationToken);
}
public class WriteRepository : IWriteRepository
{
    private readonly IWriteContext _context;

    public WriteRepository(IWriteContext context)
    {
        _context = context;
    }

    public async Task Insert(Student student, CancellationToken cancellationToken)
    {
        _context.Students.Add(student);
        await Task.CompletedTask;
    }
}
public interface IReadRepository
{
    Task<List<Student>> GetAll(CancellationToken cancellationToken);
}
public class ReadRepository : IReadRepository
{
    private readonly IReadContext _context;

    public ReadRepository(IReadContext context)
    {
        _context = context;
    }

    public async Task<List<Student>> GetAll(CancellationToken cancellationToken)
    {
        return await Task.FromResult(_context.Students.ToList());
    }
}

public interface IProducer
{
    Task Send(StudentEvent @event, CancellationToken cancellationToken);
}
public class Producer : IProducer
{
    private readonly IEventBus _eventBus;

    private readonly IReadContext _readContext;

    public Producer(IEventBus eventBus, IReadContext readContext)
    {
        _eventBus = eventBus;
        _readContext = readContext;
    }
    public async Task Send(StudentEvent @event, CancellationToken cancellationToken)
    {
        _eventBus.StudentEvents.Add(@event);
        _readContext.Students.Add(@event);
        await Task.CompletedTask;
    }
}
public interface IConsumer
{
    Task<StudentEvent> Consume(CancellationToken cancellationToken);
}
public class Consumer : IConsumer
{
    private readonly IEventBus _eventBus;

    public Consumer(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<StudentEvent> Consume(CancellationToken cancellationToken)
    {
        return await Task.FromResult(_eventBus.StudentEvents.Last());
    }
}

public class StudentEvent
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    public static implicit operator StudentEvent(Student v)
    {
        return new StudentEvent()
        {
            Id = v.Id,
            UserName = v.UserName,
            Title = v.Title,
            CreatedAt = v.CreatedAt,
        };
    }
}
public class Student
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Title { get; set; }
    public DateTime CreatedAt { get; set; }

    public static implicit operator Student(StudentEvent v)
    {
        return new Student()
        {
            Id = v.Id,
            UserName = v.UserName,
            Title = v.Title,
            CreatedAt = v.CreatedAt,
        };
    }
}