using LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.Extensions;

public static class MediatorExtensions
{
    public static async Task DispatchDomainEventAsync(this IMediator mediator, DbContext dbContext)
    {
        var domainEntities = dbContext.ChangeTracker.Entries<MetadataAggregateRoot>().Where(x => x.Entity.DomainEvents.Any());
        var domaminEvents = domainEntities.SelectMany(x => x.Entity.DomainEvents).ToImmutableList();
        domainEntities.ToImmutableList().ForEach(x => x.Entity.ClearDomainEvent());
        foreach (var domainEvent in domaminEvents)
        {
            await mediator.Publish(domainEvent);
        }
    }
}