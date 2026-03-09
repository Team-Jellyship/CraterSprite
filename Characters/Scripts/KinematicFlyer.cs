using Godot;

namespace CraterSprite.Characters.Scripts;

public partial class KinematicFlyer : CharacterBody2D
{
    [Export] private float _maxSpeed = 15.0f;
    [Export] private float _acceleration = 32.0f;
    [Export] private Vector2 _input;

    public override void _PhysicsProcess(double delta)
    {
        var deltaTime = (float)delta;
        var velocity = GetVelocity();

        if (_input.IsZeroApprox() && velocity != Vector2.Zero)
        {
            velocity = CraterMath.ScaleVectorLength(velocity, -_acceleration * deltaTime);
        }
        else
        {
            velocity = CraterMath.ClampVectorLength(velocity + _input * (_acceleration * deltaTime), _maxSpeed);
        }
        SetVelocity(velocity);
        MoveAndSlide();
    }

    public void SetMoveInput(Vector2 input)
    {
        _input = CraterMath.ClampVectorLength(input, 1.0f);
    }
}