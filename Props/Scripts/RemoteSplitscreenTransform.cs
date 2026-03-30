using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.Props;

public partial class RemoteSplitscreenTransform : Node2D
{
    [Export] public Node2D remotePath
    {
        set
        {
            if (_remotePath != null)
            {
                _remotePath.TreeExiting -= QueueFree;
            }
            _remotePath = value;
            _remotePath.TreeExiting += QueueFree;
        }
        get => _remotePath;
    }
    private Node2D _remotePath;

    public override void _EnterTree()
    {
        SetNotifyTransform(true);
    }

    private void UpdatePosition()
    {
        _remotePath?.SetGlobalPosition(CraterFunctions.GetPositionInViewport(this));
    }

    public override void _Notification(int what)
    {
        if (what == NotificationTransformChanged)
        {
            UpdatePosition();
        }
    }
}