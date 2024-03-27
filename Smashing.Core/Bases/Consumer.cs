using MessagePack;
using MongoDB.Bson;
using MongoDB.Driver;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Smashing.Core.Bases;

public static class SeriabledExtensions
{
    public static string ConvertToJson(this byte[] obj)
    {
        return MessagePackSerializer.ConvertToJson(obj);
    }

    public static T Deserialize<T>(this byte[] obj)
    {
        return MessagePackSerializer.Deserialize<T>(obj);
    }
}

[MessagePackObject]
public class SerializableEvents
{
    [Key(0)] public Guid Id { get; set; }

    [Key(1)] public required string Message { get; set; }

    public byte[] Serialize()
    {
        return MessagePackSerializer.Serialize(this);
    }
}

public static class SerializableMessageBuilder
{
    public static SerializableEvents Init<T>(T @event) where T : class
    {
        return new SerializableEvents
        {
            Id = Guid.NewGuid(),
            Message = JsonSerializer.Serialize(@event)
        };
    }
}

public class AddTaskoCommand
{
    public required string Detail { get; set; }
    public bool IsDone { get; set; }
    public DateTime DueDate { get; set; }
}

public class Tasko : BaseEntity
{
    public required string Detail { get; set; }
    public bool IsDone { get; set; }
    public DateTime DueDate { get; set; }

    public static implicit operator Tasko(AddTaskoCommand command)
    {
        return new Tasko
        {
            Detail = command.Detail,
            DueDate = command.DueDate,
            IsDone = command.IsDone
        };
    }
}
public class TaskoCreatedEvent
{
    public Guid Id { get; set; }
    public required string Detail { get; set; }
    public bool IsDone { get; set; }
    public DateTime DueDate { get; set; }

    public static implicit operator TaskoCreatedEvent(Tasko entity)
    {
        return new TaskoCreatedEvent
        {
            Id = entity.Id,
            Detail = entity.Detail,
            DueDate = entity.DueDate,
            IsDone = entity.IsDone
        };
    }
}
public class TaskoCreatedRabbitConsumer : ConsumerEvent<TaskoCreatedEvent>
{
    public TaskoCreatedRabbitConsumer(IWriteRepository<Tasko> tasko, IConnectionFactory connection) 
        : base(tasko, connection, "hello")
    {
    }
}
public class TaskoCreatedRabbitProducer : ProducerEvent<TaskoCreatedEvent>
{
    public TaskoCreatedRabbitProducer(IConnectionFactory connection) :
        base(connection, "hello")
    {
    }
}

public abstract class ProducerEvent<T> : IProducerEvent<T> where T : class
{
    private readonly IConnectionFactory connectionFactory;
    private readonly string queueName;

    protected ProducerEvent(IConnectionFactory connectionFactory, string queueName)
    {
        this.connectionFactory = connectionFactory;
        this.queueName = queueName;
    }

    public void Send(T @event, CancellationToken cancellationToken)
    {
        var serializer = SerializableMessageBuilder.Init(@event);

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queueName,
            false,
            false,
            false,
            null);

        var body = serializer.Serialize();

        channel.BasicPublish(string.Empty,
            queueName,
            null,
            body);
    }
}
public interface IProducerEvent<T> where T : class
{
    void Send(T tasko, CancellationToken cancellationToken = default);
}
public interface IConsumerEvent<T> where T : class
{
    void Consume();
}
public abstract class ConsumerEvent<T> : IConsumerEvent<T> where T : class
{
    private readonly IWriteRepository<Tasko> _repository;
    private readonly IConnectionFactory connectionFactory;
    private readonly string queueName;

    protected ConsumerEvent(IWriteRepository<Tasko> repository, IConnectionFactory connectionFactory, string queueName)
    {
        this.connectionFactory = connectionFactory;
        this.queueName = queueName;
        _repository = repository;
    }

    public void Consume()
    {
        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();
        channel.QueueDeclare(queueName,
            false,
            false,
            false,
            null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, ea) =>
        {
            var body = ea.Body.ToArray();
            Console.WriteLine($" [x] Received body {body.ConvertToJson()}");
            var mc2 = body.Deserialize<SerializableEvents>();
            var tasko = JsonSerializer.Deserialize<TaskoCreatedEvent>(mc2.Message)!;
            Console.WriteLine($" [x] Received message:\n" +
                              $" {tasko.Id}\n" +
                              $" {tasko.Detail}\n" +
                              $" {tasko.IsDone}\n" +
                              $" {tasko.DueDate}\n" +
                              $" {tasko.Detail}");
            //var filter = Builders<Tasko>.Filter.Eq(x => x.Id, tasko.Id);
            //var update = Builders<Tasko>.Update
            //    .Set(restaurant => restaurant.Detail, $"{tasko.Detail} - Resolvido")
            //    .Set(restaurant => restaurant.IsDone, true);
            Tasko taskoNew = new Tasko()
            {
                Detail = tasko.Detail,
                DueDate = tasko.DueDate,
                Id = tasko.Id,
                IsDone = true,
                UserName = "tasko",
                Title = "Title",
                CreatedAt = DateTime.Now,
            };
            _repository.UpdateAsync(taskoNew);
        };
        channel.BasicConsume(queueName,
            true,
            consumer);
        Thread.Sleep(TimeSpan.FromSeconds(5));
    }
}
public interface IConsumer
{
    Task<BaseEvent> Consume(CancellationToken cancellationToken = default);
}

public class Consumer : IConsumer
{
    private readonly IEventBus _eventBus;

    public Consumer(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public async Task<BaseEvent> Consume(CancellationToken cancellationToken = default)
    {
        return await Task.FromResult(_eventBus.StudentEvents.Last());
    }
}