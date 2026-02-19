using Godot;
using System;

namespace CraterSprite;

public partial class KinematicCharacter : CharacterBody2D
{
	private const float GravityConstant = 32.0f * 9.8f;
	private const float DefaultMaxFallSpeed = 1000.0f;
	private const float NegativeKillY = 1000.0f;

	// MOVEMENT
	[ExportGroup("Movement")]
	// How quickly the character accelerates
	[Export(PropertyHint.None, "suffix:px/s\\u00b2")]
	private float _acceleration = 50.0f;
	
	// The max speed the character can travel while on the ground
	[Export(PropertyHint.None, "suffix:px/s")]
	private float _maxSpeed = 5.0f;

	// The max speed the character can travel while on the ground and crouched
	[Export(PropertyHint.None, "suffix:px/s")]
	private float _crouchedMaxSpeed = 5.0f;

	// How quickly the character decelerates when it either has no move input,
	// or has somehow moved beyond its max speed, e.g., its aerial speed is higher
	// than its walk speed, and it just landed on the ground
	[Export(PropertyHint.None, "suffix:px/s\\u00b2")]
	private float _friction = 50.0f;
	
	[Export] private bool _isCrouched;
	
	
	// AERIAL
	[ExportGroup("Aerial")]
	// Max horizontal speed in the air
	[Export(PropertyHint.None, "suffix:px/s")]
	private float _maxAirSpeedHorizontal = 10.0f;
	
	// Max vertical speed in the air
	[Export(PropertyHint.None, "suffix:px/s")]
	private float _maxAirSpeedVertical = DefaultMaxFallSpeed;

	// Acceleration in the air, as a factor of regular acceleration
	[Export] private float _airControlFactor = 0.5f;

	// Downward acceleration due to gravity, in Gs
	// 1g is 32 * 9.8 px/s^2
	[Export(PropertyHint.None, "suffix:g")]
	private float _gravity = 1.0f;
	
	
	// JUMPING
	[ExportGroup("Jumping")]
	
	// The speed the character will travel vertically while jumping
	[Export(PropertyHint.None, "suffix:px/s")]
	private float _jumpStrength = 100.0f;

	// How many times the character can jump without landing
	// Walking off of a platform will consume one jump
	[Export] private uint _numJumps = 2;

	// Number of jumps remaining. Should not be set, this is only to
	// show in the editor
	[Export] private uint _numJumpsRemaining;
	
	// Maximum time a regular jump can be held
	[Export(PropertyHint.Range, "0,5,suffix:s")]
	private float _jumpTime = 0.5f;
	
	// Grace period after leaving a platform before the first jump is automatically consumed
	[Export(PropertyHint.Range, "0,5,suffix:s")]
	private float _coyoteTime = 0.5f;
	
	// WALL SLIDING
	[ExportGroup("Wall Sliding")]
	
	// How fast the character moves vertically when sliding up or down a wall
	[Export(PropertyHint.None, "suffix:px/s")]
	private float _maxWallSlideSpeed = 50.0f;
	
	// How fast the character decelerates when moving faster than the max wall slide speed
	[Export(PropertyHint.None, "suffix:px/s\\u00b2")]
	private float _wallFriction = 50.0f;

	// How fast the character jumps when hitting a wall
	[Export(PropertyHint.None, "suffix:px/s")]
	private float _wallJumpStrength = 100.0f;

	// The angle the character jumps off of the wall. 0 is horizontal, and 90 is directly up
	[Export(PropertyHint.Range, "0,90,1,suffix:\\u00b0")]
	private float _wallJumpAngle = 80.0f;

	// If true, the character will always get at least one jump back when hitting a wall
	[Export] private bool _restoreJumpOnHitWall = true; 
	
	// KNOCKBACK
	[ExportGroup("Knockback")]
	// The angle the character is knocked back. 0 is directly back, 90 is directly up.
	[Export(PropertyHint.Range, "0,90,1,suffix:\\u00b0")]
	private float _knockbackAngle = 45.0f;

	[Export(PropertyHint.None, "suffix:px/s")]
	private float _defaultKnockbackStrength = 200.0f;
	
	[Signal] public delegate void MoveSpeedChangedEventHandler(float moveSpeed);
	[Signal] public delegate void OnCrouchedEventHandler();
	[Signal] public delegate void OnUncrouchedEventHandler();

	[Signal] public delegate void OnClickedEventHandler();

	public CraterEvent onHitFloor = new();
	
	public float moveInput { get; private set; }

	private int _direction;
	private bool _isJumping;
	private Timer _jumpTimer = new();
	private Timer _coyoteTimer = new();
	private Vector2 _pendingImpulses;

	public override void _Ready()
	{
		AddChild(_jumpTimer);
		_jumpTimer.SetName("JumpTimer");
		_jumpTimer.OneShot = true;
		_jumpTimer.Timeout += StopJumping;
		
		AddChild(_coyoteTimer);
		_coyoteTimer.SetName("CoyoteTimer");
		_coyoteTimer.OneShot = true;
		_coyoteTimer.Timeout += () => { if (_numJumpsRemaining > 0) {--_numJumpsRemaining;} };

		InputEvent += (_, @event, _) =>
		{
			if (@event is InputEventMouseButton { Pressed: true })
			{
				EmitSignalOnClicked();
			}
		};
	}
	
	public override void _PhysicsProcess(double delta)
	{
		var currentVelocity = Velocity;
		if (moveInput == 0.0f)
		{
			currentVelocity.X = CraterMath.MoveTo(currentVelocity.X, 0.0f, GetFriction() * (float)delta);
		}
		// If the character is not moving at max speed
		else if (Math.Abs(currentVelocity.X) < GetMaxHorizontalSpeed())
		{
			// Don't allow the input based acceleration to push character beyond the max speed
			var newHorizontalSpeed = currentVelocity.X + moveInput * GetAcceleration() * (float)delta;
			if (Math.Abs(newHorizontalSpeed) > GetMaxHorizontalSpeed())
			{
				currentVelocity.X = GetMaxHorizontalSpeed() * Math.Sign(currentVelocity.X);
			}
			else
			{
				currentVelocity.X = newHorizontalSpeed;
			}
		}
		// character is trying to push back, let them influence acceleration
		else if (Math.Sign(currentVelocity.X) != Math.Sign(moveInput))
		{
			currentVelocity.X += moveInput * GetAcceleration() * (float)delta;
		}

		if (_isJumping)
		{
			currentVelocity.Y = Math.Min(-_jumpStrength, currentVelocity.Y);
		}
		else if (!IsOnFloor() && !IsOnWall())
		{
			currentVelocity.Y += GravityConstant * _gravity * (float)delta;
		}
		
		var maxSpeed = GetMaxHorizontalSpeed();
		currentVelocity.X = CraterMath.ClampTowards(currentVelocity.X, -maxSpeed, maxSpeed, GetFriction() * (float) delta);
		if (IsOnFloor())
		{
			currentVelocity.Y = Math.Clamp(currentVelocity.Y, -_maxAirSpeedVertical, _maxAirSpeedVertical);
		}
		else if (IsOnWall())
		{
			currentVelocity.Y = CraterMath.MoveTo(currentVelocity.Y, _maxWallSlideSpeed,
				_wallFriction * (float)delta);
		}

		var pendingImpulses = ConsumeImpulses();
		if (pendingImpulses != Vector2.Zero)
		{
			currentVelocity = pendingImpulses;
		}
		
		SetVelocity(currentVelocity);

		var onFloorBeforeMove = IsOnFloor();
		var onWallBeforeMove = IsOnWall();
		
		MoveAndSlide();
		if (IsOnFloor() && !onFloorBeforeMove)
		{
			Land();
		}
		else if (!IsOnFloor() && onFloorBeforeMove)
		{
			LeavePlatform();
		}

		if (IsOnWallOnly() && !onWallBeforeMove)
		{
			HitWall();
		}

		if (GlobalPosition.Y > NegativeKillY)
		{
			QueueFree();
		}
		QueueRedraw();
	}
	
	public override void _Draw()
	{
		DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, GetVelocity() * 0.25f, new Color(1.0f, 0.0f, 0.0f));
	}

	/**
	 * <summary>
	 * Set the move input value of the character. The character's
	 * acceleration scales linearly with move input. Setting this value will emit
	 * the MoveSpeedChanged signal
	 * </summary>
	 * <param name="input">
	 * The direction to move, in the range [-1, 1]
	 * </param>
	 */
	public void SetMoveInput(float input)
	{
		moveInput = Math.Clamp(input, -1.0f, 1.0f);

		if (input == 0.0f)
		{
			return;
		}
		_direction = Mathf.Sign(moveInput);
		EmitSignalMoveSpeedChanged(moveInput);
	}
	
	/**
	 * <summary>
	 * Begin a jump. The jump will end automatically after jump time,
	 * or if StopJumping is called. If the character is on the wall and
	 * not on the floor, will perform a wall jump instead.
	 * Will not jump if the character has no more jumps remaining
	 * </summary>
	 */
	public void StartJumping()
	{
		if (IsOnWallOnly())
		{
			WallJump();
		}
		else if (CanJump())
		{
			Jump();
		}
	}

	/**
	 * <summary>
	 * End a jump. Does not do anything if the jump was a wall jump
	 * </summary>
	 */
	public void StopJumping()
	{
		_isJumping = false;
	}
	
	/**
	 * <summary>
	 * Crouch down. Emits OnCrouched signal
	 * </summary>
	 */
	public void Crouch()
	{
		_isCrouched = true;

		EmitSignal(SignalName.OnCrouched);
	}

	/**
	 * <summary>
	 * Stop crouching down. Emits OnUncrouched signal
	 * </summary>
	 */
	public void Uncrouch()
	{
		_isCrouched = false;
		EmitSignal(SignalName.OnUncrouched);
	}

	/**
	 * <summary>Add a directional impulse to the character, in px/s. Will be applied on the next tick</summary>
	 */
	public void AddImpulse(Vector2 impulse)
	{
		_pendingImpulses += impulse;
	}

	/**
	 * <summary>Add a knockback impulse. Automatically assigns direction.</summary>
	 */
	public void ApplyKnockback(float velocity)
	{
		var directionVector = CraterMath.VectorFromAngle(_knockbackAngle);
		directionVector.X *= -_direction;
		directionVector *= velocity;
		AddImpulse(directionVector);
	}

	/**
	 * <summary>Add a knockback impulse with default strength. Automatically assigns direction.</summary>
	 */
	public void ApplyKnockbackDefault(float damage)
	{
		ApplyKnockback(_defaultKnockbackStrength);
	}

	/**
	 * <summary>
	 * Method called when landing on the floor. Resets jump counts and coyote time
	 * </summary>
	 */
	private void Land()
	{
		_numJumpsRemaining = _numJumps;
		_coyoteTimer.Stop();
		onHitFloor.Invoke();
	}

	private void LeavePlatform()
	{
		if (!_isJumping)
		{
			_coyoteTimer.Start(_coyoteTime);
		}
	}

	private void HitWall()
	{
		if (_restoreJumpOnHitWall)
		{
			_numJumpsRemaining = Math.Max(_numJumpsRemaining, 1);
		}
	}

	private void Jump()
	{
		_coyoteTimer.Stop();
		
		_isJumping = true;
		_jumpTimer.Start(_jumpTime);
		
		--_numJumpsRemaining;
	}

	private void WallJump()
	{
		var wallJumpVelocity = CalculateJumpVector() * _wallJumpStrength;
		wallJumpVelocity.X *= Math.Sign(GetWallNormal().X);
		Velocity = wallJumpVelocity;
	}
	
	private bool CanJump()
	{
		return _numJumpsRemaining > 0;
	}

	private float GetMaxHorizontalSpeed()
	{
		return IsOnFloor() ? (_isCrouched ? _crouchedMaxSpeed : _maxSpeed) :
			_maxAirSpeedHorizontal;
	}

	private float GetAcceleration()
	{
		return IsOnFloor() ? _acceleration : _acceleration * _airControlFactor;
	}

	private float GetFriction()
	{
		return IsOnFloor() ? _friction : _friction * _airControlFactor;
	}

	private Vector2 CalculateJumpVector()
	{
		// godot uses -y up for *reasons*, so invert the angle
		var rads = Mathf.DegToRad(-_wallJumpAngle);
		return new Vector2(Mathf.Cos(rads), Mathf.Sin(rads));
	}

	private Vector2 ConsumeImpulses()
	{
		var result = _pendingImpulses;
		_pendingImpulses = Vector2.Zero;
		return result;
	}
}
