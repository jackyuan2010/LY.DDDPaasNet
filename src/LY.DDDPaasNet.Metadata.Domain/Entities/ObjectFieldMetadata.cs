using LY.DDDPaasNet.Metadata.Domain.DomainEvents;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.Entities;

namespace LY.DDDPaasNet.Metadata.Domain.Entities;

public class ObjectFieldMetadata : MetadataAggregateRoot<string>
{
    public string ApiName { get; private set; }

    public string Description { get; private set; }

    public int Version { get; private set; }

    public ObjectFieldMetadata(string apiName, string description)
    {
        Id = Guid.NewGuid().ToString("N");
        ApiName = apiName;
        Description = description;
        Version = 1;
    }
}