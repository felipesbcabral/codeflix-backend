using FC.Codeflix.Catalog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace FC.Codeflix.Catalog.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(this IServiceCollection services)
    {
        services.AddDbConnections();
        return services;
    }

    private static IServiceCollection AddDbConnections(this IServiceCollection services)
    {
        services.AddDbContext<CodeflixCatalogDbContext>(
            options => options.UseInMemoryDatabase(
                "InMemory-DSV-Database"
            )
        );

        return services;
    }
}
