using Godot;
using System;

namespace CraterSprite
{
	public partial class KinematicCharacter : CharacterBody2D
	{
		public const float GravityConstant = 32.0f * 9.8f;
		public const float DefaultMaxFallSpeed = 1000.0f;

		[ExportGroup("Movement")]
		[Export(PropertyHint.None, "suffix:px/s\\u00b2")]
		private float _acceleration = 50.0f;
		
		[Export(PropertyHint.None, "suffix:px/s")]
		private float _maxSpeed = 5.0f;

		[Export(PropertyHint.None, "suffix:px/s")]
		private float _crouchedMaxSpeed = 5.0f;

		[Export] private bool _isCrouched = false;
		
		[ExportGroup("Aerial")]
		[Export(PropertyHint.None, "suffix:px/s")]
		private float _maxAirSpeedHorizontal = 10.0f;
		
		[Export(PropertyHint.None, "suffix:px/s")]
		private float _maxAirSpeedVertical = DefaultMaxFallSpeed;

		[Export] private float _airControlFactor = 0.5f;

		[Export(PropertyHint.None, "suffix:g")]
		private float _gravity = 1.0f;
		
		
		[ExportGroup("Jumping")]
		[Export(PropertyHint.None, "suffix:px/s")]
		private float _jumpStrength = 100.0f;

		[Export] private uint _numJumps = 2;

		[Export] private uint _numJumpsRemaining;
		
		[Export(PropertyHint.Range, "0,5,suffix:s")]
		private float _jumpTime = 0.5f;
		
		[Export(PropertyHint.Range, "0,5,suffix:s")]
		private float _coyoteTime = 0.5f;

		[Signal] public delegate void MoveSpeedChangedEventHandler(float moveSpeed);
		[Signal] public delegate void OnCrouchedEventHandler();
		[Signal] public delegate void OnUncrouchedEventHandler();
		
		private float _moveInput;
		private bool _isJumping;
		private Timer _jumpTimer = new();
		private Timer _coyoteTimer = new();

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
		}

		public void SetMoveInput(float input)
		{
			_moveInput = Math.Clamp(input, -1.0f, 1.0f);

			if (input == 0.0f)
			{
				return;
			}
			EmitSignal(SignalName.MoveSpeedChanged, _moveInput);
		}

		public override void _PhysicsProcess(double delta)
		{
			var currentVelocity = Velocity;
			if (_moveInput == 0.0f)
			{
				currentVelocity.X = CraterMath.MoveTo(currentVelocity.X, 0.0f, GetAcceleration() * (float)delta);
			}
			// If the character is not moving at max speed
			else if (Math.Abs(currentVelocity.X) < GetMaxHorizontalSpeed())
			{
				// Don't allow the input based acceleration to push character beyond the max speed
				var newHorizontalSpeed = currentVelocity.X + _moveInput * GetAcceleration() * (float)delta;
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
			else if (Math.Sign(currentVelocity.X) != Math.Sign(_moveInput))
			{
				currentVelocity.X += _moveInput * GetAcceleration() * (float)delta;
			}

			if (_isJumping)
			{
				currentVelocity.Y = Math.Min(-_jumpStrength, currentVelocity.Y);
			}
			else if (!IsOnFloor())
			{
				currentVelocity.Y += GravityConstant * _gravity * (float)delta;
			}
			
			var maxSpeed = GetMaxHorizontalSpeed();
			currentVelocity.X = CraterMath.ClampTowards(currentVelocity.X, -maxSpeed, maxSpeed, GetAcceleration() * (float) delta);
			if (IsOnFloor())
			{
				currentVelocity.Y = Math.Clamp(currentVelocity.Y, -_maxAirSpeedVertical, _maxAirSpeedVertical);
			}
			
			SetVelocity(currentVelocity);

			var onFloorBeforeMove = IsOnFloor();
			MoveAndSlide();
			if (IsOnFloor() && !onFloorBeforeMove)
			{
				Land();
			}
			else if (!IsOnFloor() && onFloorBeforeMove)
			{
				LeavePlatform();
			}
			
			QueueRedraw();
		}

		public override void _Draw()
		{
			DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, GetVelocity() * 0.25f, new Color(1.0f, 0.0f, 0.0f));
		}

		private void Land()
		{
			_numJumpsRemaining = _numJumps;
			_coyoteTimer.Stop();
		}

		private void LeavePlatform()
		{
			if (!_isJumping)
			{
				_coyoteTimer.Start(_coyoteTime);
			}
		}

		public void StartJumping()
		{
			if (!CanJump())
			{
				return;
			}
			
			_coyoteTimer.Stop();
			
			_isJumping = true;
			_jumpTimer.Start(_jumpTime);
			
			--_numJumpsRemaining;
		}

		public void StopJumping()
		{
			_isJumping = false;
		}

		public void Crouch()
		{
			_isCrouched = true;

			EmitSignal(SignalName.OnCrouched);
		}

		public void Uncrouch()
		{
			_isCrouched = false;
			EmitSignal(SignalName.OnUncrouched);
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
	}
}
