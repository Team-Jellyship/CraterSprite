using Godot;
using System;

public partial class CrawlerBody : CharacterBody2D
{
	public const float GravityConstant = 32.0f * 9.8f;
	private const float DefaultMaxFallSpeed = 1000.0f;
	private const float NegativeKillY = 1000.0f;

	// MOVEMENT
	[ExportGroup("Movement")]
	// How quickly the character accelerates
	[Export(PropertyHint.None, "suffix:px/s\\u00b2")]
	private float _acceleration = 50.0f;
	
	// The max speed the character can travel while on the ground
	[Export(PropertyHint.None, "suffix:px/s")]
	private float _maxSpeed = 100.0f;

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

	[Export]
	private float _jumpHeldGravityFactor = 0.5f;

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

	[ExportGroup("Nodes")]
	[Export] private Area2D _characterPenetrationArea;
	
	[Signal] public delegate void MoveDirectionChangedEventHandler(float moveDirection);
	[Signal] public delegate void MoveSpeedChangedEventHandler(float moveSpeed);
	[Signal] public delegate void OnCrouchedEventHandler();
	[Signal] public delegate void OnUncrouchedEventHandler();

	[Signal] public delegate void OnClickedEventHandler();

	//public CraterEvent onHitFloor = new();
	//public CraterEvent onHitWall = new();
	
	public float moveInput { get; private set; }
	
	private bool _gravitySwitch = true;
	
	private bool droped;
	private RayCast2D dropChecker;
	private RayCast2D ledgeChecker;
	private float direction;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		droped = false;
		dropChecker = GetNode<RayCast2D>("DropChecker");
		ledgeChecker = GetNode<RayCast2D>("LedgeChecker");
		direction = 1.0f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		var currentVelocity = Velocity;
		if(droped == true){
			if(IsOnFloor()){
				if(direction == 1.0f){
					currentVelocity.X = -100.0f * GetUpDirection().Y;
					currentVelocity.Y = 100.0f * GetUpDirection().X;
				}
				else{
					//currentVelocity.X = 
					currentVelocity.X = 100.0f * GetUpDirection().Y;
					currentVelocity.Y = -100.0f * GetUpDirection().X;
					//currentVelocity.X = 0.0f;
					//currentVelocity.Y = 100.0f;
				}
			}
			if(IsOnWall()){
				//if(GetWallNormal().X < 0){
					if(direction == 1.0f){
						SetUpDirection(new Vector2(GetUpDirection().Y, -GetUpDirection().X));
						ledgeChecker.SetTargetPosition(new Vector2(ledgeChecker.TargetPosition.Y,-ledgeChecker.TargetPosition.X));
					}
					else{
						SetUpDirection(new Vector2(-GetUpDirection().Y,GetUpDirection().X));
						ledgeChecker.SetTargetPosition(new Vector2(-ledgeChecker.TargetPosition.Y,ledgeChecker.TargetPosition.X));
					}
				//}
			}
			Object groundDetect = ledgeChecker.GetCollider();
			if (groundDetect is Node groundFound){
				
			}
			else{
				direction = -direction;
				ledgeChecker.SetTargetPosition(new Vector2(-ledgeChecker.TargetPosition.Y,ledgeChecker.TargetPosition.X));
			}
		}
		else{
			currentVelocity.Y = 100.0f;
			if(IsOnFloor()){
				currentVelocity.Y = 0.0f;
				droped = true;
			}
		}
		GD.Print(ledgeChecker.TargetPosition);
		QueueRedraw();
		SetVelocity(currentVelocity);
		
		MoveAndSlide();
	}
	
	private float GetMaxHorizontalSpeed()
	{
		return IsOnFloor() ? _maxSpeed : _maxAirSpeedHorizontal;
	}
	
	public override void _Draw()
	{
		//DebugHelpers.Drawing.DrawArrow(this, Vector2.Zero, GetNode<RayCast2D>("LedgeChecker").GetTargetPosition(), new Color(1.0f, 0.0f, 0.0f));
	}
	
	
	
}
