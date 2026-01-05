using Godot;

#if TOOLS

// ReSharper disable once CheckNamespace
namespace SharpDi;

[Tool]
public partial class SharpDi : EditorPlugin
{
    public override void _EnterTree()
    {
        GD.Print("SharpDi plugin loaded");
    }

    public override void _ExitTree()
    {
        GD.Print("SharpDi plugin unloaded");
    }
}
#endif
