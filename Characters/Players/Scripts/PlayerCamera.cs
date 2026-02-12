using Godot;

namespace CraterSprite;

public partial class PlayerCamera : Camera2D
{
    [Export] private uint _playerIndex;
    [Export] private float _horizontalPosition;
    [Export] private float _verticalOffset;
    
    // If true, only landing on floors above the current floor will trigger
    // the current floor position to change
    [Export] private bool _limitToFloorsAbove = true;
    
    private Node2D _target;

    private float _floorHeight;
    
    public override void _Ready()
    {
        _target = GameMode.instance.players[(int)_playerIndex];
        _target.TreeExiting += () => { _target = null; };
        _horizontalPosition = _target.GlobalPosition.X;
        _floorHeight = _target.GlobalPosition.Y;
        GlobalPosition = _target.GlobalPosition;

        if (_target is KinematicCharacter kinematicCharacter)
        {
            kinematicCharacter.onHitFloor.AddListener(TargetHitFloor);
        }
        ResetSmoothing();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_target == null)
        {
            return;
        }
        
        SetGlobalPosition(new Vector2(_horizontalPosition, Mathf.Min(_target.GlobalPosition.Y, _floorHeight)));
    }

    private void TargetHitFloor()
    {
        var newFloorPosition = _target.GlobalPosition.Y;
        _floorHeight = _limitToFloorsAbove ? Mathf.Min(newFloorPosition, _floorHeight) : newFloorPosition;
    }
}