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
		
		private float _facingDirection = 1.0f;
		
		private RayCast2D rayCast;
		private RayCast2D groundCast;
		private Timer attackTimer;
		private Timer waitTimer;
		private ProjectileLauncher gun;
		
		public override void _Ready()
		{
			rayCast = GetNode<RayCast2D>("PlayerRay");
			groundCast = GetNode<RayCast2D>("GroundRay");
			attackTimer = GetNode<Timer>("AttackTimer");
			waitTimer = GetNode<Timer>("WaitTimer");
			gun = GetNode<ProjectileLauncher>("Gun");
		}
		
		
		public override void _PhysicsProcess(double delta)
		{
			//gun.FireProjectile();
			var currentVelocity = Velocity;
			//Code that turns the enemy around
			if(IsOnWall())
			{
				_facingDirection = -_facingDirection;
				gun.SetLookHorizontal(_facingDirection);
				if(_facingDirection < 0){
					rayCast.SetTargetPosition(new Vector2(-50,0));
					groundCast.SetTargetPosition(new Vector2(-20,20));
				}
				else{
					rayCast.SetTargetPosition(new Vector2(50,0));
					groundCast.SetTargetPosition(new Vector2(20,20));
				}
				
			}
			
			//Determines if the entitiy is going to walk off an edge and turns them
			Object groundDetect = groundCast.GetCollider();
			if(IsOnFloor()){
				if(groundDetect is Node groundFound){
					//Fuck my brain is off right now I don't want to figure out the negative of this
				}
				else{
					_facingDirection = -_facingDirection;
					if(_facingDirection < 0){
						rayCast.SetTargetPosition(new Vector2(-50,0));
						groundCast.SetTargetPosition(new Vector2(-20,20));
					}
					else{
						rayCast.SetTargetPosition(new Vector2(50,0));
						groundCast.SetTargetPosition(new Vector2(20,20));
					}
				}
			}
			
			//Applies gravity when enemy is not on ground
			if(!IsOnFloor()){
				currentVelocity.Y += GravityConstant * _gravity * (float)delta;
			}
			currentVelocity.X += _facingDirection * _acceleration;
			//Sets a max Speed
			if(Math.Abs(currentVelocity.X) > _maxSpeed){
				currentVelocity.X = _maxSpeed * _facingDirection;
			}

			Object enemyInMySight = rayCast.GetCollider();
			if(enemyInMySight is Node collidedNode){
				if(string.Equals(collidedNode.GetParent().Name, "Player") || string.Equals(collidedNode.GetParent().Name, "@CharacterBody2D@5")){
					if(attackTimer.GetTimeLeft() == 0.0 && waitTimer.GetTimeLeft() == 0.0){
						attackTimer.Start();
						waitTimer.Start();
						currentVelocity.X = 0;
						gun.FireProjectile();
					}
					else if (attackTimer.GetTimeLeft() != 0.0 && waitTimer.GetTimeLeft() != 0.0){
						currentVelocity.X = 0;
						GD.Print(attackTimer.GetTimeLeft().ToString("F2"));
					}
				}
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
			DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, GetNode<RayCast2D>("PlayerRay").GetTargetPosition(), new Color(1.0f, 0.0f, 0.0f));
			DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, GetNode<RayCast2D>("GroundRay").GetTargetPosition(), new Color(1.0f, 0.0f, 1.0f));
		}
	}
}
