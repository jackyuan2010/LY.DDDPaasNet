using LY.DDDPaasNet.Core.DependencyInjection;
using LY.DDDPaasNet.EntityFrameworkCore;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.EntityFrameworkCore;

public class MetadataDbContext : EFCoreDbContext<MetadataDbContext>
{
    public IMediator Mediator => LazyServiceProvider.LazyGetRequiredService<IMediator>();

    public MetadataDbContext(DbContextOptions<MetadataDbContext> options, ILazyServiceProvider lazyServiceProvider)
        : base(options, lazyServiceProvider)
    {
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        try
        {
            if(Mediator != null)
            {
                await Mediator.DispatchDomainEventAsync(this);
            }
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new DbUpdateConcurrencyException(ex.Message, ex);
        }
        finally
        {
            ChangeTracker.AutoDetectChangesEnabled = true;
        }
    }
}