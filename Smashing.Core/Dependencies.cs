using Microsoft.Extensions.DependencyInjection;
using Smashing.Core.Bases;
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
            .AddScoped<IWriteRepository<BaseEntity>, WriteRepository<BaseEntity>>()
            .AddScoped<IReadRepository<BaseEntity>, ReadRepository<BaseEntity>>()
            .AddScoped<IProducer, Producer>()
            .AddScoped<IConsumer, Consumer>();
        services.AddMovements();
        return services;
    }

    //public static IServiceCollection AddContexts(this IServiceCollection services, string? mysSqlConnectionString)
    //{
    //    services.AddDbContext<AppDbContext>(options =>
    //        options.UseMySql(mysSqlConnectionString, ServerVersion.AutoDetect(mysSqlConnectionString)));
    //    return services;
    //}
}