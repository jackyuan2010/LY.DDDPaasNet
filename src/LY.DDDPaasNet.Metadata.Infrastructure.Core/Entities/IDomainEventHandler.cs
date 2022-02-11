using MediatR;

namespace LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;

public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
}