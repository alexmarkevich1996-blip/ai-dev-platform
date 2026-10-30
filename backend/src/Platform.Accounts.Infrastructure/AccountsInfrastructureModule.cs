using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        // and no other module can name the concrete type.
        services.Scan(scan => scan
            .FromAssemblyOf<AccountsDbContext>()
            .AddClasses(classes => classes.Where(IsModuleImplementation), publicOnly: false)
            .AsImplementedInterfaces(IsModuleContract)
            .WithScopedLifetime());

        return services;
    }

    private static bool IsModuleImplementation(Type type) => Array.Exists(type.GetInterfaces(), IsModuleContract);

    private static bool IsModuleContract(Type contract) =>
        string.Equals(contract.Namespace, AbstractionsNamespace, StringComparison.Ordinal);
}
