using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.Props;

public partial class RemoteSplitscreenTransform : Node2D
{
    [Export] private Node2D _remotePath;
    
    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = GetGlobalMousePosition();
        _remotePath?.SetGlobalPosition(CraterFunctions.GetPositionInViewport(this));
    }
}