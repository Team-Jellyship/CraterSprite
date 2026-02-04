using Godot;
using System;

namespace CraterSprite
{
	public partial class KinematicCharacter : CharacterBody2D
	{
		public const float Gravity = 32.0f * 9.8f;

		[Export(PropertyHint.None, "suffix:px/s\\u00b2")]
		private float _acceleration = 10.0f;
		
		[Export(PropertyHint.None, "suffix:px/s")]
		private float _speed = 5.0f;

		private float _moveInput;

		public override void _Ready()
		{
			
		}

		public void SetMoveInput(float input)
		{
			_moveInput = Math.Clamp(input, -1.0f, 1.0f);
		}

		public override void _PhysicsProcess(double delta)
		{
			var currentVelocity = Velocity;
			currentVelocity.X = Math.Clamp(currentVelocity.X + _moveInput * _acceleration * (float)delta, -_speed, _speed);
			SetVelocity(currentVelocity);
			MoveAndSlide();
		}
	}
}
