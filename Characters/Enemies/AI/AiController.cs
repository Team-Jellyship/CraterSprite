using Godot;

namespace CraterSprite.Characters.Enemies.AI.Scripts;

public partial class AiController : Node2D
{
    [Export] protected Node2D target;
    
    public void SetTarget(Node2D player)
    {
        target = player;
        player.TreeExited += () =>
        {
            target = null;
            OnTargetDeath();
        };
    }

    protected virtual void OnTargetDeath()
    {
    }
}