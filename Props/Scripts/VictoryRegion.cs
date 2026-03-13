using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.Props;

public partial class VictoryRegion : Area2D
{
    public override void _EnterTree()
    {
        AreaEntered += Overlap;
    }

    private void Overlap(Area2D otherArea)
    {
        var playerState = CraterFunctions.GetNodeByClassFromRoot<PlayerState>(otherArea);
        if (playerState == null)
        {
            return;
        }

        var playerIndex = playerState.playerIndex;
        GameMode.instance.SetWinner(playerIndex);
    }
}