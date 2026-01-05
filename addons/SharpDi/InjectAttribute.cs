using System;

// ReSharper disable once CheckNamespace
namespace SharpDi;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class InjectAttribute : Attribute
{
    internal string Key { get; private set;  } = string.Empty;

    public InjectAttribute()
    {
    }

    public InjectAttribute(string key)
    {
        Key = key;
    }
}
