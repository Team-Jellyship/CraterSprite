using System;
using Godot;

namespace CraterSprite.Props;

public partial class KinematicPickup : StaticBody2D
{
    [Export] public float gravityMultiplier = 1.0f;
    [Export] private bool _canBounce = true;
    [Export] private bool _grounded = false;

    [Export] private float _dampingFactor = 0.7f;
    [Export] private float _minimumFloorY = 0.9f;
    
    // If the Y velocity's magnitude is less than this value, 
    [Export] private float _minimumYVelocity = 4.0f;

    private Vector2 _velocity;
    private Vector2 _pendingImpulses;

    public override void _PhysicsProcess(double delta)
    {
        var deltaTime = (float)delta;

        if (!_grounded)
        {
            _velocity.Y += KinematicCharacter.GravityConstant * gravityMultiplier * deltaTime;
        }
        _velocity += ConsumeImpulses();

        var result = MoveAndCollide(_velocity * deltaTime);
        if (result == null)
        {
            QueueRedraw();
            return;
        }

        _velocity = _canBounce ? Bounce(_velocity, result.GetNormal()) : result.GetRemainder();
        
        if (-_velocity.Y < _minimumYVelocity && -result.GetNormal().Y > _minimumFloorY)
        {
            _velocity = Vector2.Zero;
            _grounded = true;
        }
        
        QueueRedraw();
    }
    
    public override void _Draw()
    {
        DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, _velocity * 0.25f, new Color(1.0f, 0.0f, 0.0f));
    }

    public void AddImpulse(Vector2 impulse)
    {
        _pendingImpulses += impulse;
    }

    private Vector2 ConsumeImpulses()
    {
        var result = _pendingImpulses;
        _pendingImpulses = Vector2.Zero;
        return result;
    }

    private Vector2 Bounce(Vector2 velocity, Vector2 normal)
    {
        var reflectedVelocity = velocity.Dot(normal) * normal;
        var planarVelocity = velocity - reflectedVelocity;
        return (reflectedVelocity * -_dampingFactor) + planarVelocity;
    }
}