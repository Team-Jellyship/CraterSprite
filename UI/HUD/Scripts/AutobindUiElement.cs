using Godot;

namespace CraterSprite.UI.HUD;

public partial class AutobindUiElement : Node
{
    [Export] private int _playerIndex;
    
    public override void _EnterTree()
    {
        GameMode.instance.onPlayerSpawned.AddListener(PlayerSpawned);
    }

    public override void _ExitTree()
    {
        GameMode.instance.onPlayerSpawned.RemoveListener(PlayerSpawned);
    }

    private void PlayerSpawned(int index, Node2D playerRoot)
    {
        if (index != _playerIndex)
        {
            return;
        }

        var playerState = CraterFunctions.GetNodeByClass<PlayerState>(playerRoot);
        if (playerState != null)
        {
            Bind(playerState);
        }
    }

    protected virtual void Bind(PlayerState playerState)
    {}
}