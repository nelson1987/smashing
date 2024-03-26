using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Smashing.Core.Bases;

namespace Smashing.Core.Features.Movements;

public static class Dependencies
{
    public static IServiceCollection AddMovements(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionFactory, ConnectionFactory>(x => new ConnectionFactory
        {
            HostName = "localhost",
            Port = 5672
        })
            .AddScoped<IProducerEvent<TaskoCreatedEvent>, TaskoCreatedRabbitProducer>()
            .AddScoped<IConsumerEvent<TaskoCreatedEvent>, TaskoCreatedRabbitConsumer>()
            .AddScoped<IWriteRepository<Movement>, MovementWriteRepository>()
            .AddSingleton<IWriteRepository<Tasko>, TaskoMongoRepository>()
            .AddScoped<IReadRepository<Movement>, MovementReadRepository>()
            .AddScoped<IValidator<AddMovementCommand>, AddMovementCommandValidator>()
            .AddScoped<IAddMovementCommandHandler, AddMovementCommandHandler>()
            .AddScoped<IHttpExternalServiceClient, HttpExternalServiceClient>();
        return services;
    }

    //public static IServiceCollection AddContexts(this IServiceCollection services, string? mysSqlConnectionString)
    //{
    //    services.AddDbContext<AppDbContext>(options =>
    //        options.UseMySql(mysSqlConnectionString, ServerVersion.AutoDetect(mysSqlConnectionString)));
    //    return services;
    //}
}