namespace LY.DDDPaasNet.Core.Exceptions;

public class EntityNotFoundException : LYDDDPaasNetException
{
    public Type EntityType { get; set; }

    public object Id { get; set; }

    public EntityNotFoundException()
    {
    }

    public EntityNotFoundException(Type entityType)
        : this(entityType, null, null)
    {
    }

    public EntityNotFoundException(Type entityType, object id)
        : this(entityType, id, null)
    {
    }

    public EntityNotFoundException(Type entityType, object id, Exception innerException)
        : base(
            id == null
                ? $"There is no such an entity given id. Entity type: {entityType.FullName}"
                : $"There is no such an entity. Entity type: {entityType.FullName}, id: {id}",
            innerException)
    {
        EntityType = entityType;
        Id = id;
    }

    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    public EntityNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}