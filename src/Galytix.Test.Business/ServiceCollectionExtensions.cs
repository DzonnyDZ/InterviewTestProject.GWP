using Galytix.Test.Business.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Galytix.Test.Business;

/// <summary>Provides extension methods to be used during application startup / configuration phase</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>Registers business layer interfaces and their implementations</summary>
    /// <param name="services">Service collection to register the interfaces with</param>
    /// <returns><paramref name="services"/> to allow fluent API chaining</returns>
    public static IServiceCollection AddBusinessLayer(this IServiceCollection services)
    {
        if (services is null) throw new ArgumentNullException(nameof(services));

        services.AddSingleton<ILobBusiness, LobBusiness>();

        return services;
    }
}
