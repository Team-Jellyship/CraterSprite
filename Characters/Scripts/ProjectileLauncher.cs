using System;
using Godot;

namespace CraterSprite;
public partial class ProjectileLauncher : Node2D
{
    [Export] private PackedScene _projectile;

    [Export(PropertyHint.None, "suffix:px/s")] private float _projectileSpeed;
    [Export] private KinematicCharacter _kinematicOwner;
    [Export(PropertyHint.None, "suffix:px")] private float _offset = 0.0f;

    [Export]
    public Vector2 facingDirection
    {
        set
        {
            _facingDirection = value.Normalized();
        }
        get => _facingDirection;
    }
    private Vector2 _facingDirection;

    private CharacterStats _characterStats;

    public override void _Ready()
    {
        _characterStats = CraterFunctions.GetNodeByClassFromRoot<CharacterStats>(this);
    }

    public override void _Draw()
    {
        DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, _facingDirection * 25.0f, new Color(0.0f, 1.0f, 0.0f));
    }

    public override void _Process(double delta)
    {
        QueueRedraw();
    }

    public void FireProjectile()
    {
        var projectile = CraterFunctions.CreateInstance<Projectile>(this, _projectile, GlobalPosition + _facingDirection * _offset);
        if (projectile == null)
        {
            return;
        }

        projectile.SetOwner(_characterStats);
        projectile.velocity = _facingDirection * _projectileSpeed;
        projectile.velocity.X += _kinematicOwner?.Velocity.X ?? 0.0f;
    }

    public void SetLookHorizontal(float x)
    {
        if (x == 0.0f)
        {
            return;
        }

        _facingDirection.X = Math.Sign(x);
    }
}
