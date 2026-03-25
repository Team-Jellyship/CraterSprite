using System;
using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite;

public enum AimDirection
{
	Horizontal,
	Diagonal,
	Vertical
}

public partial class ProjectileLauncher : Node2D
{
	[Export] private PackedScene _projectile;

	[Export(PropertyHint.None, "suffix:px/s")] private float _projectileSpeed;
	[Export] private KinematicCharacter _kinematicOwner;
	[Export(PropertyHint.None, "suffix:px")] private float _offset = 0.0f;
	[Export(PropertyHint.None, "suffix:px")] private float _offsetVerticalFacing = 0.0f;
	
	// Should this projectile launcher include the parent velocity of its kinematic owner
	[Export] private bool _inheritVelocity = false;

	[Export]
	public Vector2 facingDirection
	{
		set => _facingDirection = value.Normalized();
		get => _facingDirection;
	}
	private Vector2 _facingDirection;

	[Signal] public delegate void OnAimDirectionChangedEventHandler(AimDirection aimDirection);
		
	public bool aimingUp = false;
	public bool aimingDiagonal = false;
	
	private CharacterStats _characterStats;

	public override void _Ready()
	{
		_characterStats = CraterFunctions.GetNodeByClassFromRoot<CharacterStats>(this);
		if (_kinematicOwner != null)
		{
			_kinematicOwner.MoveDirectionChanged += speed => { _facingDirection.X = Mathf.Sign(speed); };
		}
	}

	public override void _Draw()
	{
		if (!GameMode.instance.showingDebug)
		{
			return;
		}
		
		DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, _facingDirection * 25.0f, new Color(0.0f, 1.0f, 0.0f));
	}

	public override void _Process(double delta)
	{
		QueueRedraw();
	}

	public void FireProjectile()
	{
		var projectileSpawnLocation = GlobalPosition + GetFacingDirection() * _offset;
		if (aimingUp)
		{
			projectileSpawnLocation.X += _offsetVerticalFacing * MathF.Sign(facingDirection.X);
		}
		var projectile = CraterFunctions.CreateInstance<Projectile>(_projectile, projectileSpawnLocation);
		if (projectile == null)
		{
			return;
		}

		projectile.SetOwner(_characterStats);
		projectile.velocity =  GetFacingDirection() * _projectileSpeed;

		if (_inheritVelocity)
		{
			projectile.velocity.X += _kinematicOwner?.Velocity.X ?? 0.0f;
		}
	}

	public void SetAimDiagonal(bool aimingDiagonalIn)
	{
		aimingDiagonal = aimingDiagonalIn;
		EmitSignalOnAimDirectionChanged(GetAimDirection());
	}

	public void SetAimVertical(bool aimingVerticalIn)
	{
		aimingUp = aimingVerticalIn;
		EmitSignalOnAimDirectionChanged(GetAimDirection());
	}

	public void SetLookHorizontal(float x)
	{
		if (x == 0.0f)
		{
			return;
		}

		_facingDirection.X = Math.Sign(x);
	}

	private Vector2 GetFacingDirection()
	{
		return GetAimDirection() switch
		{
			AimDirection.Horizontal => _facingDirection,
			AimDirection.Diagonal => new Vector2(0.8509f * MathF.Sign(_facingDirection.X), -0.8509f),
			AimDirection.Vertical => Vector2.Up,
			_ => throw new ArgumentOutOfRangeException()
		};
	}

	private AimDirection GetAimDirection()
	{
		if (aimingDiagonal)
		{
			return AimDirection.Diagonal;
		}

		return aimingUp ? AimDirection.Vertical : AimDirection.Horizontal;
	}
}
