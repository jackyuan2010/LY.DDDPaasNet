using System.Data;

namespace LY.DDDPaasNet.Domain.Abstractions.Uow;

public interface IUnitOfWorkOptions
{
    bool IsTransactional { get; }

    IsolationLevel? IsolationLevel { get; }

    /// <summary>
    /// Milliseconds
    /// </summary>
    int? Timeout { get; }
}