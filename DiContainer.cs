using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Godot;

namespace SharpDi;

public partial class DiContainer : Node
{
    public static DiContainer Instance { get; private set; }
    private static readonly Dictionary<Type, object> _services = new();

    public override void _EnterTree()
    {
        if (Instance != null)
        {
            GD.PrintErr("DiContainer instance already exists!");
            QueueFree();
            return;
        }

        Instance = this;
    }

    public override void _ExitTree() => _services.Clear();

    public static void Add<T>(T service)
    {
        var type = typeof(T);
        if (_services.ContainsKey(type))
        {
            GD.Print($"!!! Service of type {type} is already registered. Overwriting...");
            _services[type] = service;
        }
        else
        {
            _services.Add(type, service);
        }
    }

    public static T Get<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service;
        }

        throw new Exception($"Service of type {type} not found in DiContainer.");
    }

    public void Inject(object target)
    {
        if (target is null)
        {
            return;
        }

        var targetType = target.GetType();
        var fields = targetType
            .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => x.GetCustomAttribute<InjectAttribute>() != null);

        foreach (var field in fields)
        {
            var serviceType = field.FieldType;
            if (_services.TryGetValue(serviceType, out var service))
            {
                field.SetValue(target, service);
            }
            else
            {
                GD.PushError("Service of type " + serviceType + " not found for injection into " + targetType);
            }
        }

        var properties = targetType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(x => x.GetCustomAttribute<InjectAttribute>() != null);
        foreach (var property in properties)
        {
            var serviceType = property.PropertyType;
            if (_services.TryGetValue(serviceType, out var service))
            {
                property.SetValue(target, service);
            }
            else
            {
                GD.PushError("Service of type " + serviceType + " not found for injection into " + targetType);
            }
        }
    }
}
