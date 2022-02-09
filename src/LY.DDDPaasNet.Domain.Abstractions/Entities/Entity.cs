namespace LY.DDDPaasNet.Domain.Abstractions.Entities;

public abstract class Entity : IEntity
{
    private int? hashCode;

    protected Entity()
    {
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[ENTITY: {GetType().Name}], Keys = {string.Join(",", GetKeys())}";
    }

    public abstract object[] GetKeys();

    public bool Equals(Entity other)
    {
        return EntityHelper.EntityEquals(this, other);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Entity))
        {
            return false;
        }

        return Equals((Entity)obj);
    }

    public override int GetHashCode()
    {
        if (!EntityHelper.HasDefaultKeys(this))
        {
            if (!hashCode.HasValue)
            {
                hashCode = GetKeys().Select(x => x != null ? x.GetHashCode() : 0).Aggregate((x, y) => x ^ y) ^ 31;
            }
            return hashCode.Value;
        }
        return base.GetHashCode();
    }

    public static bool operator ==(Entity left, Entity right)
    {
        return EntityHelper.EntityEquals(left, right);
    }

    public static bool operator !=(Entity left, Entity right)
    {
        return !(left == right);
    }

}

public abstract class Entity<TKey> : Entity, IEntity<TKey>
{
    public virtual TKey Id { get; protected set; }

    protected Entity()
    {

    }

    protected Entity(TKey id)
    {
        Id = id;
    }

    public override object[] GetKeys()
    {
        return new object[] { Id };
    }

    public override string ToString()
    {
        return $"[ENTITY: {GetType().Name}], Id = {Id}";
    }
}