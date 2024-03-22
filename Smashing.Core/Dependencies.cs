using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Smashing.Core.Features.Movements;

namespace Smashing.Core;

public static class Dependencies
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        var mongoConn = "mongodb://root:example@localhost:27017/";
        var mongoDbConnRead = "read";
        var mongoDbConnWrite = "write";
        services.AddSingleton<IWriteContext, WriteContext>(x => new WriteContext(mongoConn, mongoDbConnRead))
            .AddSingleton<IReadContext, ReadContext>(x => new ReadContext(mongoConn, mongoDbConnWrite))
            .AddSingleton<IEventBus, EventBus>()
            .AddScoped<IWriteRepository, WriteRepository>()
            .AddScoped<IReadRepository, ReadRepository>()
            .AddScoped<IProducer, Producer>()
            .AddScoped<IConsumer, Consumer>()
            .AddScoped<IValidator<AddMovementCommand>, AddMovementCommandValidator>()
            .AddScoped<IAddMovementCommandHandler, AddMovementCommandHandler>();
        return services;
    }

    //public static IServiceCollection AddContexts(this IServiceCollection services, string? mysSqlConnectionString)
    //{
    //    services.AddDbContext<AppDbContext>(options =>
    //        options.UseMySql(mysSqlConnectionString, ServerVersion.AutoDetect(mysSqlConnectionString)));
    //    return services;
    //}
}

public interface IWriteContext
{
    IMongoCollection<BaseEntity> Students { get; }
}

public class WriteContext : IWriteContext
{
    public WriteContext(string connectionString, string databaseName)
    {
        try
        {
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            throw new Exception("Não foi possível se conectar com o servidor.", ex);
        }
    }
    private readonly IMongoDatabase _database;
    public IMongoCollection<BaseEntity> Students => _database.GetCollection<BaseEntity>(nameof(BaseEntity));
}

public interface IReadContext
{
    IMongoCollection<BaseEntity> Students { get; }
}

public class ReadContext : IReadContext
{
    public ReadContext(string connectionString, string databaseName)
    {
        try
        {
            var mongoClient = new MongoClient(connectionString);
            _database = mongoClient.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            throw new Exception("Não foi possível se conectar com o servidor.", ex);
        }
    }
    private readonly IMongoDatabase _database;
    public IMongoCollection<BaseEntity> Students => _database.GetCollection<BaseEntity>(nameof(BaseEntity));
}

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

public interface IWriteRepository
{
    Task Insert(BaseEntity student, CancellationToken cancellationToken);
}

public class WriteRepository : IWriteRepository
{
    private readonly IWriteContext _context;

    public WriteRepository(IWriteContext context)
    {
        _context = context;
    }

    public async Task Insert(BaseEntity student, CancellationToken cancellationToken)
    {
        await _context.Students.InsertOneAsync(student);
    }
}

public interface IReadRepository
{
    Task<List<BaseEntity>> GetAll(CancellationToken cancellationToken);
}

public class ReadRepository : IReadRepository
{
    private readonly IReadContext _context;

    public ReadRepository(IReadContext context)
    {
        _context = context;
    }

    public async Task<List<BaseEntity>> GetAll(CancellationToken cancellationToken)
    {
        return await _context.Students.Find(_ => true).ToListAsync(cancellationToken);
    }
}

public interface IProducer
{
    Task Send(BaseEvent @event, CancellationToken cancellationToken);
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

    public async Task Send(BaseEvent @event, CancellationToken cancellationToken)
    {
        _eventBus.StudentEvents.Add(@event);
        await _readContext.Students.InsertOneAsync(@event);
    }
}

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