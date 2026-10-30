using Microsoft.Extensions.DependencyInjection;

namespace Platform.Integrations;

/// <summary>
/// The platform's link to external services. An empty shell in the first version.
/// </summary>
/// <remarks>
/// Per ADR-0001 the platform neither grants nor revokes Repository Access in GitHub,
/// so there is nothing here to register yet. The project exists so that the first
/// Integration lands inside a boundary that is already set, rather than being spread
/// through Accounts and Projects. It owns no storage and therefore has no
/// Infrastructure project.
/// </remarks>
public static class IntegrationsModule
{
    public static IServiceCollection AddIntegrationsModule(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);
        return services;
    }
}
