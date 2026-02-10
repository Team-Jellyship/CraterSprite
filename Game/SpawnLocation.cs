using Godot;

namespace CraterSprite;

public partial class SpawnLocation : Node2D
{
    [Export] public uint defaultIndex { private set; get; }

    public override void _EnterTree()
    {
        GameMode.instance.AddSpawnLocation(this);
    }
}