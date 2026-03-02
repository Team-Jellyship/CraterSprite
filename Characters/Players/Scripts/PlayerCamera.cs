using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite;

public partial class PlayerCamera : Camera2D
{
    [Export] private uint _playerIndex;
    
    private Node2D _target;
    public override void _EnterTree()
    {
        GameMode.instance.onPlayerSpawned.AddListener(PlayerSpawned);
    }

    private void PlayerSpawned(int playerIndex, Node2D player)
    {
        if (playerIndex != _playerIndex)
        {
            return;
        }
        
        var cameraAttachmentPoint = CraterFunctions.FindNodeByClass<PlayerCameraAttachmentPoint>(player);
        cameraAttachmentPoint?.SetRemoteNode(GetPath());

        ResetSmoothing();
    }
}