using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Godot;

// ReSharper disable once CheckNamespace
namespace SharpDi;

public partial class DiContainer : Node
{
    public static DiContainer Instance { get; private set; }
    private static readonly Dictionary<Type, List<object>> _services = new();
    private static readonly Dictionary<(string, Type), List<object>> _keyedServices = new();

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

    public override void _ExitTree()
    {
        _services.Clear();
        _keyedServices.Clear();
    }

    public static void Add<T>(T service)
    {
        var type = typeof(T);
        if (!_services.ContainsKey(type))
        {
            _services[type] = new List<object>();
        }

        _services[type].Add(service);
    }

    public static void AddKeyed<T>(string key, T service)
    {
        var type = typeof(T);
        var dictKey = (key, type);
        if (!_keyedServices.ContainsKey(dictKey))
        {
            _keyedServices[dictKey] = new List<object>();
        }
        _keyedServices[dictKey].Add(service);
    }

    public static T Get<T>()
    {
        var type = typeof(T);
        if (_services.TryGetValue(type, out var service))
        {
            return (T)service.Last();
        }

        throw new Exception($"Service of type {type} not found in DiContainer.");
    }

    public static List<T> GetAll<T>()
    {
        var type = typeof(T);
        return _services.TryGetValue(type, out var services) ? services.Cast<T>().ToList() : throw new Exception($"Service of type {type} not found in DiContainer.");
    }

    public static List<T> GetAllKeyed<T>(string key)
    {
        var type = typeof(T);
        var dictKey = (key, type);
        if (_keyedServices.TryGetValue(dictKey, out var services))
        {
            return services.Cast<T>().ToList();
        }

        throw new Exception($"Service of type {type} with key '{key}' not found in DiContainer.");
    }

    public static T GetKeyed<T>(string key)
    {
        var type = typeof(T);
        var dictKey = (key, type);
        if (_keyedServices.TryGetValue(dictKey, out var services))
        {
            return (T)services.Last();
        }

        throw new Exception($"Service of type {type} with key '{key}' not found in DiContainer.");
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
            var attr = field.GetCustomAttribute<InjectAttribute>();
            if (!string.IsNullOrWhiteSpace(attr.Key) && _keyedServices.TryGetValue((attr.Key, serviceType), out var serviceList))
            {
                field.SetValue(target, serviceList.Last());
            }
            else if (_services.TryGetValue(serviceType, out var service))
            {
                field.SetValue(target, service.Last());
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
            var attr = property.GetCustomAttribute<InjectAttribute>();

            if (!string.IsNullOrWhiteSpace(attr.Key) && _keyedServices.TryGetValue((attr.Key, serviceType), out var serviceList))
            {
                property.SetValue(target, serviceList.Last());
            }
            else if (_services.TryGetValue(serviceType, out var service))
            {
                property.SetValue(target, service.Last());
            }
            else
            {
                GD.PushError("Service of type " + serviceType + " not found for injection into " + targetType);
            }
        }
    }
}
