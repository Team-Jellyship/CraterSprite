using System;
using CraterSprite.DebugHelpers;
using Godot;

namespace CraterSprite;

public partial class KinematicEnemy : CharacterBody2D
{

    public const float GravityConstant = 32.0f * 9.8f;
    public const float DefaultMaxFallSpeed = 1000.0f;
    public const float NegativeKillY = 1000.0f;

    // MOVEMENT
    [ExportGroup("Movement")]
    [Export(PropertyHint.None, "suffix:px/s\\u00b2")]
    private float _acceleration = 25.0f;

    [Export] private float _airControlFactor = 0.5f;
    private Timer _attackTimer;

    [Export(PropertyHint.None, "suffix:px/s")]
    private float _crouchedMaxSpeed = 5.0f;

    private float _facingDirection = 1.0f;

    [Export(PropertyHint.None, "suffix:px/s\\u00b2")]
    private float _friction = 50.0f;

    [Export(PropertyHint.None, "suffix:g")]
    private float _gravity = 1.0f;

    private RayCast2D _groundCast;
    private ProjectileLauncher _gun;

    // AERIAL
    [ExportGroup("Aerial")]
    [Export(PropertyHint.None, "suffix:px/s")]
    private float _maxAirSpeedHorizontal = 10.0f;

    [Export(PropertyHint.None, "suffix:px/s")]
    private float _maxAirSpeedVertical = DefaultMaxFallSpeed;

    [Export(PropertyHint.None, "suffix:px/s")]
    private float _maxSpeed = 100.0f;

    private RayCast2D _rayCast;
    private Timer _waitTimer;

    public override void _Ready()
    {
        _rayCast = GetNode<RayCast2D>("PlayerRay");
        _groundCast = GetNode<RayCast2D>("GroundRay");
        _attackTimer = GetNode<Timer>("AttackTimer");
        _gun = GetNode<ProjectileLauncher>("Gun");
    }


    public override void _PhysicsProcess(double delta)
    {
        //gun.FireProjectile();
        var currentVelocity = Velocity;
        //Code that turns the enemy around
        if (IsOnWall())
        {
            _facingDirection = -_facingDirection;
            _gun.SetLookHorizontal(_facingDirection);
            if (_facingDirection < 0)
            {
                _rayCast.SetTargetPosition(new Vector2(-50, 0));
                _groundCast.SetTargetPosition(new Vector2(-20, 20));
            }
            else
            {
                _rayCast.SetTargetPosition(new Vector2(50, 0));
                _groundCast.SetTargetPosition(new Vector2(20, 20));
            }

        }

        //Determines if the entity is going to walk off an edge and turns them
        if (IsOnFloor())
        {
            var floorEdge = _groundCast.GetCollider();
            if (floorEdge == null)
            {
                _facingDirection = -_facingDirection;
                if (_facingDirection < 0)
                {
                    _rayCast.SetTargetPosition(new Vector2(-50, 0));
                    _groundCast.SetTargetPosition(new Vector2(-20, 20));
                }
                else
                {
                    _rayCast.SetTargetPosition(new Vector2(50, 0));
                    _groundCast.SetTargetPosition(new Vector2(20, 20));
                }
            }
        }

        //Applies gravity when enemy is not on ground
        if (!IsOnFloor())
        {
            currentVelocity.Y += GravityConstant * _gravity * (float)delta;
        }
        currentVelocity.X += _facingDirection * _acceleration;
        //Sets a max Speed
        if (Math.Abs(currentVelocity.X) > _maxSpeed)
        {
            currentVelocity.X = _maxSpeed * _facingDirection;
        }



        //Sets our temporary Variable into active in the game
        SetVelocity(currentVelocity);
        //Draws the Raycast Arrows
        QueueRedraw();
        //Physical Engine goes BRRRRR
        MoveAndSlide();
    }


    public override void _Draw()
    {
        Drawing.DrawArrow(this, Vector2.Zero, GetNode<RayCast2D>("PlayerRay").GetTargetPosition(), new Color(1.0f, 0.0f, 0.0f));
        Drawing.DrawArrow(this, Vector2.Zero, GetNode<RayCast2D>("GroundRay").GetTargetPosition(), new Color(1.0f, 0.0f, 1.0f));
    }
}