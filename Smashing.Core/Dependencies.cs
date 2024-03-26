using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Smashing.Core.Bases;
using Smashing.Core.Features.Movements;
using Smashing.Core.Features.Users;

namespace Smashing.Core;

public static class Dependencies
{
    public static IServiceCollection AddDependencies(this IServiceCollection services)
    {
        var connectionString = "mongodb://root:example@localhost:27017/";
        var client = new MongoClient(connectionString);

        services
            .AddSingleton<IMongoWriteContextOptions, MongoWriteContextOptions>(x => new MongoWriteContextOptions
            {
                Database = "warehouse",
                MongoClient = client
            })
            .AddSingleton<IMongoReadContextOptions, MongoReadContextOptions>(x => new MongoReadContextOptions
            {
                Database = "sales",
                MongoClient = client
            })
            .AddSingleton<IWriteContext, WriteContext>()
            .AddScoped<IReadContext, ReadContext>()
            .AddSingleton<IEventBus, EventBus>();
        services.AddMovements();
        services.AddUserAuthentication();
        return services;
    }
}