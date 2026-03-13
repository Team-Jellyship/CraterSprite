using Godot;

namespace CraterSprite.Characters.Enemies.AI.Scripts;

public partial class WalkingController : AiController
{
    [Export] private float _gunCooldownTime = 2.0f;
    
    // Exporting Node types allows you to assign them in the editor, and avoids having to hardcode the paths,
    // in the event you want to change the node path or name, you won't need to modify the source code.
    // Nodes can still be null, and I've omitted null checking them because godot will catch this itself,
    // so you have to be sure to assign them
    [Export] private ProjectileLauncher _gun;
    [Export] private KinematicCharacter _character;
    [Export] private RayCast2D _playerDetection;
    [Export] private RayCast2D _groundCast;

    private float _facingDirection = 1.0f;
    private bool _attackOnCooldown;
    private Timer _attackTimer;
    
    public override void _Ready()
    {
        // CreateTimer is a helper function to automatically... create a timer :)
        // It just saves on repeating code, so I used it here
        _attackTimer = CraterFunctions.CreateTimer(this, "AttackTimer", () => _attackOnCooldown = false);
        
        // Register our flip function with the character's HitWall event, so it will automatically be run,
        // and we don't have to check each frame for it
        _character.onHitWall.AddListener(Flip);
        
        // KinematicCharacter will automatically move if there is an input given, either from a player or from an Ai Controller,
        // so set the default movement input. This will change later, as the enemy moves around
        _character.SetMoveInput(_facingDirection);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_playerDetection.GetCollider() != null && !_attackOnCooldown)
        {
            _gun.FireProjectile();
            _attackTimer.Start(_gunCooldownTime);
            _attackOnCooldown = true;
        }
        
        // Because the wall hit logic is moved into an event instead, this method doesn't do anything when it's not on the floor
        // To avoid nesting the code (indents and adding blocks), I just used an early return statement.
        // If the character isn't on the floor, this exits early!
        if (!_character.IsOnFloor())
        {
            return;
        }
        
        // Determines if the entity is going to walk off an edge and turns them
        // Godot's raycast can return null when it doesn't hit anything, so it's faster than trying to cast
        if (_groundCast.GetCollider() == null || _playerDetection.GetCollider() != null || _character.IsOnWall())
        {
            Flip();
        }
    }

    // Flip the relevant character pieces around, including our continuous raycasts, gun, and character input
    private void Flip()
    {
        // Invert the facing direction
        _facingDirection *= -1.0f;
        
        // FacingDirection is really just carrying the sign of our controller's direction, i.e., negative is left, positive is right,
        // so we can avoid branching and just multiply everything instead
        _playerDetection.SetTargetPosition(new Vector2(50.0f * _facingDirection, 0.0f));
        _groundCast.SetTargetPosition(new Vector2(20.0f * _facingDirection, 20));
        _gun.facingDirection = new Vector2(_facingDirection, 0.0f);
        
        // Feed the facing direction back into the kinematic character so it can start moving in the new direction
        // This is identical to how a player would move if they changed direction
        _character.SetMoveInput(_facingDirection);
    }
}