using Godot;

namespace CraterSprite.Props;

public partial class Gem : Area2D
{
    [Export] private float _chargeAmount = 20.0f;
    
    public override void _Ready()
    {
        AreaEntered += Overlap;
    }

    private void Overlap(Area2D area)
    {
        var playerState = CraterFunctions.GetNodeByClassFromRoot<PlayerState>(area);
        if (playerState == null)
        {
            return;
        }
        
        playerState.AddSuperCharge(_chargeAmount);
        QueueFree();
    }
}