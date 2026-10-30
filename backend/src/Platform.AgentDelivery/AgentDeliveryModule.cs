using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Platform.AgentDelivery.Authentication;
using Platform.AgentDelivery.Tools;

namespace Platform.AgentDelivery;

/// <summary>
/// Registers the MCP server and the Agent Token scheme with the composition root.
/// </summary>
/// <remarks>
/// The MCP server runs on a route of the same host as the HTTP API, so a browser and
/// an Employee Agent reach one process and one set of rules. It is not a separate
/// service and it does not speak stdio.
/// </remarks>
public static class AgentDeliveryModule
{
    /// <summary>The route the MCP server answers on.</summary>
    public const string RoutePattern = "/mcp";

    public static IServiceCollection AddAgentDelivery(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentAgentEmployee, CurrentAgentEmployee>();
        services.AddScoped<EmployeeProjectTools>();

        services
            .AddMcpServer()
            .WithHttpTransport()
            .WithTools<EmployeeProjectTools>();

        services.AddAuthorizationBuilder()
            .AddPolicy(AgentTokenDefaults.Policy, policy => policy
                .AddAuthenticationSchemes(AgentTokenDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser());

        return services;
    }

    /// <summary>Adds the Agent Token scheme alongside the browser's cookie scheme.</summary>
    public static AuthenticationBuilder AddAgentTokenAuthentication(this AuthenticationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.AddScheme<AuthenticationSchemeOptions, AgentTokenAuthenticationHandler>(
            AgentTokenDefaults.AuthenticationScheme,
            configureOptions: null);
    }

    /// <summary>Maps the MCP route. Only a request carrying a valid Agent Token reaches it.</summary>
    public static IEndpointConventionBuilder MapAgentDelivery(this IEndpointRouteBuilder endpoints)
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        return endpoints
            .MapMcp(RoutePattern)
            .RequireAuthorization(AgentTokenDefaults.Policy);
    }
}
