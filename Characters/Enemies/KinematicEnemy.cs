using Godot;
using System;

namespace CraterSprite
{
	public partial class KinematicEnemy : CharacterBody2D
	{
		
		public const float GravityConstant = 32.0f * 9.8f;
		public const float DefaultMaxFallSpeed = 1000.0f;
		public const float NegativeKillY = 1000.0f;
		
		// MOVEMENT
		[ExportGroup("Movement")]
		[Export(PropertyHint.None, "suffix:px/s\\u00b2")]
		private float _acceleration = 25.0f;
		
		[Export(PropertyHint.None, "suffix:px/s")]
		private float _maxSpeed = 100.0f;

		[Export(PropertyHint.None, "suffix:px/s")]
		private float _crouchedMaxSpeed = 5.0f;

		[Export(PropertyHint.None, "suffix:px/s\\u00b2")]
		private float _friction = 50.0f;
		
		// AERIAL
		[ExportGroup("Aerial")]
		[Export(PropertyHint.None, "suffix:px/s")]
		private float _maxAirSpeedHorizontal = 10.0f;
		
		[Export(PropertyHint.None, "suffix:px/s")]
		private float _maxAirSpeedVertical = DefaultMaxFallSpeed;

		[Export] private float _airControlFactor = 0.5f;

		[Export(PropertyHint.None, "suffix:g")]
		private float _gravity = 1.0f;
		
		private float _movingDirection = 1.0f;
		
		public float moveInput { get; private set; }
		
		public override void _PhysicsProcess(double delta)
		{
			//Variable used to store the Velocity
			var currentVelocity = Velocity;
			if(IsOnWall())
			{
				_movingDirection = -_movingDirection;
			}
			//Applies gravity when enemy is not on ground
			if(!IsOnFloor()){
				currentVelocity.Y += GravityConstant * _gravity * (float)delta;
			}
			
			currentVelocity.X += _movingDirection * _acceleration;
			//Sets a max Speed
			if(Math.Abs(currentVelocity.X) > _maxSpeed){
				currentVelocity.X = _maxSpeed * _movingDirection;
			}
			
			var rayCast = GetNode<RayCast2D>("PlayerRay");
			
			if(rayCast.IsColliding()){
				//currentVelocity.X -= _movingDirection * _acceleration;
				//if(Math.Abs(currentVelocity.X) < 0){
				//	currentVelocity.X = 0.0f;
				//}
				currentVelocity.X = 0.0f;
				
			}
			
			
			GD.Print(currentVelocity.X);
			
			//Sets our temporary Variable into active in the game
			SetVelocity(currentVelocity);
			
			//Physical Engine goes BRRRRR
			MoveAndSlide();
			
			
		}
		
		public void SetMoveInput(float input)
		{
			moveInput = Math.Clamp(input, -1.0f, 1.0f);
		}
		
		
		public override void _Draw()
		{
			DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, GetVelocity() * 0.25f, new Color(1.0f, 0.0f, 0.0f));
		}
	}
}
