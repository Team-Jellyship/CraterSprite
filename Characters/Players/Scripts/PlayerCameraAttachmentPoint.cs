using Godot;

namespace CraterSprite;

public partial class PlayerCameraAttachmentPoint : RemoteTransform2D
{
    [Export] private float _horizontalPosition;
    [Export] private float _verticalOffset;
    
    // If true, only landing on floors above the current floor will trigger
    // the current floor position to change
    [Export] private bool _limitToFloorsAbove = true;
    private float _floorHeight;

    private Node2D _parent;

    public override void _EnterTree()
    {
        _parent = GetParent<Node2D>();
        if (_parent is KinematicCharacter character)
        {
            character.onHitFloor.AddListener(TargetHitFloor);
        }
        _floorHeight = _parent.GlobalPosition.Y;
        _horizontalPosition = _parent.GlobalPosition.X;
    }

    public override void _PhysicsProcess(double delta)
    {
        SetGlobalPosition(new Vector2(_horizontalPosition, Mathf.Min(_parent.GlobalPosition.Y, _floorHeight)));
    }

    private void TargetHitFloor()
    {
        var newFloorPosition = _parent.GlobalPosition.Y;
        _floorHeight = _limitToFloorsAbove ? Mathf.Min(newFloorPosition, _floorHeight) : newFloorPosition;
    }
}