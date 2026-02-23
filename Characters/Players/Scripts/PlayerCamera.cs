using Godot;

namespace CraterSprite;

public partial class PlayerCamera : Camera2D
{
    [Export] private uint _playerIndex;
    
    private Node2D _target;
    public override void _Ready()
    {
        var player = GameMode.instance.players[(int)_playerIndex];

        var cameraAttachmentPoint = CraterFunctions.FindNodeByClass<PlayerCameraAttachmentPoint>(player);
        cameraAttachmentPoint?.SetRemoteNode(GetPath());

        ResetSmoothing();
    }
}