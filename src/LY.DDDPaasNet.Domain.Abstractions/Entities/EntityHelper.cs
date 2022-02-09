namespace LY.DDDPaasNet.Domain.Abstractions.Entities;

public static class EntityHelper
{
    public static bool EntityEquals(IEntity left, IEntity right)
    {
        if (left == null || right == null)
        {
            return false;
        }

        //Same instances must be considered as equal
        if (ReferenceEquals(left, right))
        {
            return true;
        }

        //Must have a IS-A relation of types or must be same type
        var typeOfLeft = left.GetType();
        var typeOfRight = right.GetType();
        if (!typeOfLeft.IsAssignableFrom(typeOfRight) && !typeOfRight.IsAssignableFrom(typeOfLeft))
        {
            return false;
        }

        //Transient objects are not considered as equal
        if (HasDefaultKeys(left) && HasDefaultKeys(right))
        {
            return false;
        }

        var leftKeys = left.GetKeys();
        var rightKeys = right.GetKeys();

        if (leftKeys.Length != rightKeys.Length)
        {
            return false;
        }

        for (var i = 0; i < leftKeys.Length; i++)
        {
            var leftKey = leftKeys[i];
            var rightKey = rightKeys[i];

            if (leftKey == null)
            {
                if (rightKey == null)
                {
                    continue;
                }
                return false;
            }

            if (rightKey == null)
            {
                return false;
            }

            if (IsDefaultKeyValue(leftKey) && IsDefaultKeyValue(rightKey))
            {
                return false;
            }

            if (!leftKey.Equals(rightKey))
            {
                return false;
            }
        }

        return true;
    }

    public static bool HasDefaultKeys(IEntity entity)
    {
        if (entity == null)
        {
            return true;
        }

        foreach (var key in entity.GetKeys())
        {
            if (!IsDefaultKeyValue(key))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsDefaultKeyValue(object value)
    {
        if (value == null)
        {
            return true;
        }

        var type = value.GetType();

        if (type == typeof(int))
        {
            return Convert.ToInt32(value) <= 0;
        }

        if (type == typeof(long))
        {
            return Convert.ToInt64(value) <= 0;
        }

        if (type.IsValueType)
        {
            return value.Equals(Activator.CreateInstance(type));
        }


        return false;
    }

    public static bool HasDefaultId<TKey>(IEntity<TKey> entity)
    {
        if (EqualityComparer<TKey>.Default.Equals(entity.Id, default))
        {
            return true;
        }

        if (typeof(TKey) == typeof(int))
        {
            return Convert.ToInt32(entity.Id) <= 0;
        }

        if (typeof(TKey) == typeof(long))
        {
            return Convert.ToInt64(entity.Id) <= 0;
        }

        return false;
    }


}