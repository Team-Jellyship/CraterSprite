using CraterSprite.Shared.Scripts;
using Godot;

namespace CraterSprite.Props;

public partial class Gem : Area2D, IDamageListener
{
    [Export] private float _chargeAmount = 20.0f;
    
    public override void _Ready()
    {
        AreaEntered += Overlap;
    }

    public void TakeDamage(float damageAmount, CharacterStats source)
    {
        if (source is PlayerState playerState)
        {
            GameMode.instance.NotifyGemDestroyed(playerState.index);
        }
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