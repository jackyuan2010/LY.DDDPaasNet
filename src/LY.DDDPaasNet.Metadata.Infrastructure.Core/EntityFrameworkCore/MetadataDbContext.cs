using LY.DDDPaasNet.Core.DependencyInjection;
using LY.DDDPaasNet.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.EntityFrameworkCore;

public class MetadataDbContext : EFCoreDbContext<MetadataDbContext>
{
    public MetadataDbContext(DbContextOptions<MetadataDbContext> options, ILazyServiceProvider lazyServiceProvider)
        : base(options, lazyServiceProvider)
    {
    }
}