using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Smashing.Core.Bases;

namespace Smashing.Core.Features.Movements;

public static class Dependencies
{
    public static IServiceCollection AddMovements(this IServiceCollection services)
    {
        services
            .AddScoped<IWriteRepository<Movement>, MovementWriteRepository>()
            .AddScoped<IReadRepository<Movement>, MovementReadRepository>()
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