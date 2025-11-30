# SharpDi

SharpDi is a lightweight dependency injection helper for Godot C# projects. It provides an easy way to register services once and inject them into nodes using a simple `[Inject]` attribute.

## Features
- Tiny DI container built on top of Godot nodes
- Attribute-based injection for fields and properties
- Editor plugin that initializes the container when enabled

## Installation
1. Copy the `addons/SharpDi` folder into your Godot project.
2. In the Godot editor, open **Project > Project Settings > Plugins**.
3. Enable **SharpDi**.

## Usage
1. Add the `DiContainer` node to your root scene or autoload it.
2. Register services at startup:
   ```csharp
   public override void _EnterTree()
   {
       base._EnterTree();
       DiContainer.Add<IScoreService>(this);
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
4. Call `DiContainer.Add` inside `_EnterTree` (or an earlier initialization hook) and perform injections in `_Ready`. This order avoids cases where dependencies are still null because the container was not populated when `_Ready` runs on other nodes.
5. The `DiContainer` will populate all annotated fields and properties with registered services.

## Requirements
- Godot 4.x
- .NET 6 / C# 10 or compatible mono build

## Contributing
Issues and pull requests are welcome. Please open an issue describing the feature or bug before submitting a PR.
