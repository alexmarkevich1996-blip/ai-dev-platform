using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Platform.Accounts.Abstractions;
using Platform.Accounts.Infrastructure.Invitations;
using Platform.Accounts.Infrastructure.Persistence;

namespace Platform.Accounts.Infrastructure;

/// <summary>Registers the Accounts module's storage with the composition root.</summary>
public static class AccountsInfrastructureModule
{
    /// <summary>The namespace holding the contracts this module implements for others.</summary>
    private const string AbstractionsNamespace = "Platform.Accounts.Abstractions";

    public static IServiceCollection AddAccountsInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<AccountsDbContext>(options => options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable(
                AccountsDbContext.MigrationHistoryTable,
                AccountsDbContext.Schema))
            .UseSnakeCaseNamingConvention());

        // Implementations stay internal: the composition root wires them by contract,
        // and no other module can name the concrete type. The invitation sender is
        // left out because which one applies depends on the environment.
        services.Scan(scan => scan
            .FromAssemblyOf<AccountsDbContext>()
            .AddClasses(classes => classes.Where(IsEnvironmentIndependentImplementation), publicOnly: false)
            .AsImplementedInterfaces(IsModuleContract)
            .WithScopedLifetime());

        return services;
    }

    /// <summary>
    /// Delivers Invitations by writing the link to the application log.
    /// </summary>
    /// <remarks>
    /// For Development only. The first version has no SMTP and no mail container, so
    /// this is how an Invitation is accepted locally. Registering it anywhere else
    /// would put working accept links in the log.
    /// </remarks>
    public static IServiceCollection AddDevelopmentInvitationSender(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddScoped<IInvitationSender, LoggingInvitationSender>();
        return services;
    }

    private static bool IsEnvironmentIndependentImplementation(Type type) =>
        IsModuleImplementation(type) && type != typeof(LoggingInvitationSender);

    private static bool IsModuleImplementation(Type type) => Array.Exists(type.GetInterfaces(), IsModuleContract);

    private static bool IsModuleContract(Type contract) =>
        string.Equals(contract.Namespace, AbstractionsNamespace, StringComparison.Ordinal);
}
