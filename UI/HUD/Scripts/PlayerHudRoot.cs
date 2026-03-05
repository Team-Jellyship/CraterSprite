using Godot;

namespace CraterSprite.UI.HUD.Scripts;

public partial class PlayerHudRoot : Control
{
    [Export] private int _playerIndex;
    
    public override void _EnterTree()
    {
        ChildEnteredTree += (node =>
        {
            node.Set("_playerIndex", _playerIndex);
        });
    }
}