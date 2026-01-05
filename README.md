# SharpDi

![SharpDi Logo](Logo-min.png)

SharpDi is a lightweight dependency injection helper for Godot C# projects. It provides an easy way to register services once and inject them into nodes using a simple `[Inject]` attribute.

## Features
- Tiny DI container built on top of Godot nodes
- Attribute-based injection for fields and properties
- Support for keyed services (multiple instances of the same type)
- Editor plugin that initializes the container when enabled
- Simple static API for service registration and retrieval

## Installation
1. Copy the `addons/SharpDi` folder into your Godot project.
2. In the Godot editor, open **Project > Project Settings > Plugins**.
3. Enable **SharpDi**.

## Usage

### Basic Service Registration & Injection

1. Add the `DiContainer` node to your root scene or autoload it.

2. Register services at startup using `_EnterTree()`:
   ```csharp
   public override void _EnterTree()
   {
       base._EnterTree();
       DiContainer.Add<IScoreService>(new ScoreService());
   }
   ```

3. Inject dependencies into any node by adding the `[Inject]` attribute:
   ```csharp
   public partial class Player : Node
   {
       [Inject]
       private IScoreService _scoreService;

       public override void _Ready()
       {
           DiContainer.Instance.Inject(this);
       }
   }
   ```

### Keyed Services

For scenarios where you need multiple instances of the same type, use keyed services:

```csharp
// Registration with keys
public override void _EnterTree()
{
    base._EnterTree();
    DiContainer.AddKeyed<IDataService>("database", new DbDataService());
    DiContainer.AddKeyed<IDataService>("cache", new CacheDataService());
}
```

```csharp
// Injection with keys
public partial class DataManager : Node
{
    [Inject("database")]
    private IDataService _dbService;

    [Inject("cache")]
    private IDataService _cacheService;

    public override void _Ready()
    {
        DiContainer.Instance.Inject(this);
    }
}
```

### Manual Service Retrieval

You can also retrieve services manually:

```csharp
// Get single service (returns the last registered instance)
var scoreService = DiContainer.Get<IScoreService>();

// Get all services of a type
var allServices = DiContainer.GetAll<IScoreService>();

// Get keyed services
var dbService = DiContainer.GetKeyed<IDataService>("database");
var allDbServices = DiContainer.GetAllKeyed<IDataService>("database");
```

## Best Practices

- **Use `_EnterTree()` for registration**: Register services in `_EnterTree()` instead of `_Ready()` to ensure they are available before other nodes are initialized.
- **Use `_Ready()` for injection**: Perform dependency injections in `_Ready()` to avoid null references when the container hasn't been populated yet.
- **This order prevents initialization issues**: Dependencies might still be null because the container wasn't fully populated when `_Ready()` runs on other nodes.
- **Injection works on fields and properties**: Both private and public fields/properties can be injected using the `[Inject]` attribute.

## API Reference

### DiContainer Static Methods

| Method | Description |
|--------|-------------|
| `Add<T>(T service)` | Register a service of type T |
| `AddKeyed<T>(string key, T service)` | Register a keyed service |
| `Get<T>()` | Get the last registered service of type T |
| `GetAll<T>()` | Get all registered services of type T |
| `GetKeyed<T>(string key)` | Get the last registered service with a key |
| `GetAllKeyed<T>(string key)` | Get all services with a specific key |
| `Inject(object target)` | Inject all marked dependencies into the target object |

### InjectAttribute

```csharp
[Inject]  // Injects the service of matching type
private IScoreService _scoreService;

[Inject("myKey")]  // Injects the keyed service
private IDataService _dataService;
```

## Requirements
- Godot 4.x
- .NET 6 / C# 10 or compatible mono build

## Contributing
Issues and pull requests are welcome. Please open an issue describing the feature or bug before submitting a PR.

## License
SharpDi is distributed under the MIT License. See `LICENSE` for details.
