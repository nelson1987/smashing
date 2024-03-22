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
        var mongoDbConnRead = "sales";
        var mongoDbConnWrite = "sales";
        services.AddSingleton<IWriteContext, WriteContext>(x => new WriteContext(mongoConn, mongoDbConnRead))
            .AddSingleton<IReadContext, ReadContext>(x => new ReadContext(mongoConn, mongoDbConnWrite))
            .AddSingleton<IEventBus, EventBus>()
            .AddScoped<IWriteRepository<Movement>, MovementWriteRepository>()
            .AddScoped<IReadRepository<Movement>, MovementReadRepository>()
            .AddScoped<IWriteRepository<BaseEntity>, WriteRepository<BaseEntity>>()
            .AddScoped<IReadRepository<BaseEntity>, ReadRepository<BaseEntity>>()
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
    IMongoDatabase Database { get; }
}

public class WriteContext : IWriteContext
{
    private readonly MongoClient _mongoClient;
    public WriteContext(string connectionString, string databaseName)
    {
        try
        {
            _mongoClient = new MongoClient(connectionString);
            Database = _mongoClient.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            throw new Exception("Não foi possível se conectar com o servidor.", ex);
        }
    }

    public IMongoDatabase Database { get; }
}

public interface IReadContext
{
    IMongoDatabase Database { get; }
}

public class ReadContext : IReadContext
{
    private readonly MongoClient _mongoClient;
    public ReadContext(string connectionString, string databaseName)
    {
        try
        {
            _mongoClient = new MongoClient(connectionString);
            Database = _mongoClient.GetDatabase(databaseName);
        }
        catch (Exception ex)
        {
            throw new Exception("Não foi possível se conectar com o servidor.", ex);
        }
    }

    public IMongoDatabase Database { get; }
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

public interface IWriteRepository<in T> where T : BaseEntity
{
    Task CreateAsync(T newBook, CancellationToken cancellationToken = default);

    Task UpdateAsync(T updatedBook, CancellationToken cancellationToken = default);
}
public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
{
    private readonly IWriteContext _context;
    private IMongoCollection<T> _collection => _context.Database.GetCollection<T>(nameof(T));
    public WriteRepository(IWriteContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(T newBook, CancellationToken cancellationToken = default) =>
        await _collection.InsertOneAsync(newBook);

    public async Task UpdateAsync(T updatedBook, CancellationToken cancellationToken = default) =>
        await _collection.ReplaceOneAsync(x => x.Id == updatedBook.Id, updatedBook);
}

public interface IReadRepository<T> where T : BaseEntity
{
    Task<List<T>> GetAsync(CancellationToken cancellationToken = default);
    Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default);
}

public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
{
    private readonly IReadContext _context;
    private IMongoCollection<T> _collection => _context.Database.GetCollection<T>(nameof(T));

    public ReadRepository(IReadContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAsync(CancellationToken cancellationToken = default) =>
        await _collection.Find(_ => true).ToListAsync(cancellationToken);

    public async Task<T?> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);
}

public interface IProducer
{
    Task Send(BaseEvent @event, CancellationToken cancellationToken);
}

public class Producer : IProducer
{
    private readonly IEventBus _eventBus;

    private readonly IWriteRepository<BaseEntity> _readContext;

    public Producer(IEventBus eventBus, IWriteRepository<BaseEntity> readContext)
    {
        _eventBus = eventBus;
        _readContext = readContext;
    }

    public async Task Send(BaseEvent @event, CancellationToken cancellationToken)
    {
        _eventBus.StudentEvents.Add(@event);
        await _readContext.CreateAsync(@event);
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