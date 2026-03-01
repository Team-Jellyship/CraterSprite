using System;
using CraterSprite.Characters.Scripts;
using Godot;

namespace CraterSprite.Characters.Enemies.AI.Scripts;

public partial class FlyingController : Node2D
{
    [Export] private KinematicFlyer _character;
    [Export] private Node2D _target;
    
    private Vector2 _flyingOffset;
    private float _offsetTime;

    public override void _EnterTree()
    {
        GameMode.instance.onPlayerSpawned.AddListener(PlayerSpawned);
    }

    public override void _ExitTree()
    {
        GameMode.instance.onPlayerSpawned.RemoveListener(PlayerSpawned);
    }

    public override void _Ready()
    {
        _offsetTime += GD.Randf() * 6.0f;
    }

    public override void _Process(double delta)
    {
        _offsetTime += (float)delta;
        _flyingOffset.Y = Mathf.Sin(_offsetTime * 2.0f) * 32.0f - 16.0f;
        
        if (_target == null)
        {
            return;
        }

        _flyingOffset.X = MathF.Sign(GetTargetLocation().X - GlobalPosition.X) * 64.0f;
        _character.SetMoveInput(GetTargetLocation() - _character.GlobalPosition);
        QueueRedraw();
    }

    public override void _Draw()
    {
        DrawCircle(GetTargetLocation() - GlobalPosition, 8.0f, new Color(0.0f, 1.0f, 0.0f, 0.25f));
    }

    private void PlayerSpawned(int i, Node2D player)
    {
        // I'll probably move this hook somewhere else
        if (i != 0)
        {
            return;
        }

        SetTarget(player);
    }

    private void SetTarget(Node2D player)
    {
        _target = player;
        player.TreeExited += () =>
        {
            _target = null;
            _character.SetMoveInput(Vector2.Zero);
        };
    }

    private Vector2 GetTargetLocation()
    {
        if (_target == null)
        {
            return _character.GlobalPosition;
        }

        return _target.GlobalPosition + _flyingOffset;
    }
}
