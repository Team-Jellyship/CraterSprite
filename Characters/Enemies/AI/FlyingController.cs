using System;
using CraterSprite.Characters.Scripts;
using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.Characters.Enemies.AI.Scripts;

public partial class FlyingController : AiController
{
    [Export] private KinematicFlyer _character;
    
    private Vector2 _flyingOffset;
    private float _offsetTime;

    public override void _Ready()
    {
        _offsetTime += GD.Randf() * 6.0f;
    }

    public override void _Process(double delta)
    {
        _offsetTime += (float)delta;
        _flyingOffset.Y = Mathf.Sin(_offsetTime * 2.0f) * 32.0f - 16.0f;
        
        if (target == null)
        {
            return;
        }

        _flyingOffset.X = MathF.Sign(GetTargetLocation().X - GlobalPosition.X) * 64.0f;
        _character.SetMoveInput(GetTargetLocation() - _character.GlobalPosition);
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (!GameMode.instance.showingDebug)
        {
            return;
        }
        DrawCircle(GetTargetLocation() - GlobalPosition, 8.0f, new Color(0.0f, 1.0f, 0.0f, 0.25f));
    }

    protected override void OnTargetDeath()
    {
        _character.SetMoveInput(Vector2.Zero);
    }

    private Vector2 GetTargetLocation()
    {
        if (target == null)
        {
            return _character.GlobalPosition;
        }

        return target.GlobalPosition + _flyingOffset;
    }
}
