using LY.DDDPaasNet.Metadata.Domain.Entities;
using LY.DDDPaasNet.Metadata.Infrastructure.Core.Repositories;

namespace LY.DDDPaasNet.Metadata.Domain.Repositories;

public interface IObjectMetadataRepository : IMetadataRepository<ObjectMetadata, string>
{
}