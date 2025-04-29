using Microsoft.Extensions.DependencyInjection;
using MicroMLAst.Core.Parsing;

namespace MicroMLAst.Core;

/// <summary>DI helper so callers can write services.AddMicroML();</summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicroML(this IServiceCollection services)
    {
        // register Parser as a singleton implementation of IParser
        services.AddSingleton<IParser, Parser>();
        return services;
    }
}
