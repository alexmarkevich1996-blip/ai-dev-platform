using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Platform.Projects.Infrastructure.Persistence;

namespace Platform.Projects.Infrastructure;

/// <summary>Registers the Projects module's storage with the composition root.</summary>
public static class ProjectsInfrastructureModule
{
    /// <summary>The namespace holding the contracts this module implements for others.</summary>
    private const string AbstractionsNamespace = "Platform.Projects.Abstractions";

    public static IServiceCollection AddProjectsInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionString);

        services.AddDbContext<ProjectsDbContext>(options => options
            .UseNpgsql(connectionString, npgsql => npgsql.MigrationsHistoryTable(
                ProjectsDbContext.MigrationHistoryTable,
                ProjectsDbContext.Schema))
            .UseSnakeCaseNamingConvention());

        services.Scan(scan => scan
            .FromAssemblyOf<ProjectsDbContext>()
            .AddClasses(classes => classes.Where(IsModuleImplementation), publicOnly: false)
            .AsImplementedInterfaces(IsModuleContract)
            .WithScopedLifetime());

        return services;
    }

    private static bool IsModuleImplementation(Type type) => Array.Exists(type.GetInterfaces(), IsModuleContract);

    private static bool IsModuleContract(Type contract) =>
        string.Equals(contract.Namespace, AbstractionsNamespace, StringComparison.Ordinal);
}
