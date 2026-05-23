using System.Collections.Generic;
using System.Linq;

namespace Istapio.Domain.Entities.Common;

public abstract class BaseJunctionEntity
{
    protected abstract IReadOnlyCollection<object?> GetEqualityComponents();

    public override bool Equals(object? obj)
    {
        if (obj is not BaseJunctionEntity other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var component in GetEqualityComponents())
            hash.Add(component);

        return hash.ToHashCode();
    }
}