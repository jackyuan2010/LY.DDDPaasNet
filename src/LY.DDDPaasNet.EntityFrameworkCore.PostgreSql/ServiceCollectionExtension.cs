using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
namespace LY.DDDPaasNet.EntityFrameworkCore.PostgreSql;

public static class ServiceCollectionExtension
{
    public static IServiceCollection UseNpgsqlDomainDbContext<TDbContext>(this IServiceCollection services, string connectionString)
        where TDbContext : EFCoreDbContext<TDbContext>
    {
        return UseNpgsqlDomainDbContext<TDbContext>(services, builder => builder.UseNpgsql(connectionString));
    }

    private static IServiceCollection UseNpgsqlDomainDbContext<TDbContext>(IServiceCollection services, Action<DbContextOptionsBuilder> actionOptions)
        where TDbContext : EFCoreDbContext<TDbContext>
    {
        return services.AddDbContext<TDbContext>(actionOptions);
    }

    public static IServiceCollection UseNpgsqlDomainDbContext<TDbContext>(this IServiceCollection services, string connectionString, Action<NpgsqlDbContextOptionsBuilder> npgsqlOptionsAction)
        where TDbContext : EFCoreDbContext<TDbContext>
    {
        return UseNpgsqlDomainDbContext<TDbContext>(services, builder => builder.UseNpgsql(connectionString, npgsqlOptionsAction));
    }
}