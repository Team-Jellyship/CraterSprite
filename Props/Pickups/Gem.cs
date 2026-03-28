using CraterSprite.Game.GameMode;
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

    public void TakeDamage(float damageAmount, CharacterStats source, bool canBeBlocked)
    {
        if (source is PlayerState playerState)
        {
            var offset = GlobalPosition - playerState.match3Spawner.GlobalPosition;
            GameMode.instance.NotifyGemDestroyed(playerState.playerIndex, offset);
            GD.Print($"[Gem] Gem '{Name}' was destroyed by player {playerState.playerIndex} with relative position {offset}");
        }
        
        Owner.QueueFree();
    }

    private void Overlap(Area2D area)
    {
        var playerState = CraterFunctions.GetNodeByClassFromRoot<PlayerState>(area);
        if (playerState == null)
        {
            return;
        }
        
        playerState.AddSuperCharge(_chargeAmount);
        Owner.QueueFree();
    }
}