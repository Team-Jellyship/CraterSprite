using System;
using CraterSprite.Game.GameMode;
using CraterSprite.Props;
using Godot;

namespace CraterSprite;

public partial class SpecialProjectileLauncher : Node2D
{
	[Export] private PackedScene _projectile;
	[Export] private PackedScene _remoteProjectile;

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
	[Signal] public delegate void OnFiredEventHandler();
		
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
		if (_characterStats is not PlayerState playerState)
		{
			return;
		}
		var playerData = GameMode.instance.playerData[playerState.playerIndex];
		var projectile = _remoteProjectile.Instantiate<Projectile>();
		projectile.GlobalPosition = CraterFunctions.GetGlobalOverlayPosition(GlobalPosition, playerData.camera);
		GameMode.instance.sharedRoot.AddChild(projectile);
		
		projectile.velocity =  GetFacingDirection() * _projectileSpeed;
		EmitSignalOnFired();
		
		if (_inheritVelocity)
		{
			projectile.velocity.X += _kinematicOwner?.Velocity.X ?? 0.0f;
		}
		
		var projectileColliderMask = CraterFunctions.CreateInstance<Projectile>(_projectile, GlobalPosition);
		projectileColliderMask.SetOwner(_characterStats);
		var remote = CraterFunctions.GetNodeByClass<RemoteSplitscreenTransform>(projectile);
		if (remote != null)
		{
			remote.remotePath = projectileColliderMask;
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
