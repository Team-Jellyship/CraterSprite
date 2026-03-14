using System;
using CraterSprite.Teams;
using Godot;

namespace CraterSprite.Characters.Enemies.AI.Scripts;

public enum FloorEdgeResponse
{
    Flip,
    Jump,
    Ignore
}

public partial class WalkingController : AiController
{
    [Export] private float _gunCooldownTime = 2.0f;
    [Export] private TeamFilter _gunTeamFilter;

    // Exporting Node types allows you to assign them in the editor, and avoids having to hardcode the paths,
    // in the event you want to change the node path or name, you won't need to modify the source code.
    // Nodes can still be null, and I've omitted null checking them because godot will catch this itself,
    // so you have to be sure to assign them
    [Export] private ProjectileLauncher _gun;
    [Export] private KinematicCharacter _character;
    [Export] private RayCast2D _playerDetection;
    [Export] private RayCast2D _groundCast;

    [Export] private FloorEdgeResponse _floorEdgeResponse;

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
        if (!_attackOnCooldown && ShouldShootTarget(_playerDetection.GetCollider()))
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
        // Godot's raycast can return null when it doesn't hit anything, so it's faster than trying to cast.
        // Also, make sure to check again if the character is on the wall, because otherwise the player will
        // be able to force the enemy into the wall, where it will no longer trigger the event from above.
        if (_playerDetection.GetCollider() != null || _character.IsOnWall())
        {
            Flip();
        }
        // The priority of execution is to check if this controller can see a player, then check if it's still on the wall
        // only then do we fall back to jumping etc.
        else if (_groundCast.GetCollider() == null)
        {
            switch (_floorEdgeResponse)
            {
                case FloorEdgeResponse.Flip:
                    Flip();
                    break;

                case FloorEdgeResponse.Jump:
                    _character.StartJumping();
                    break;

                default:
                case FloorEdgeResponse.Ignore:
                    break;
            }

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

    private bool ShouldShootTarget(GodotObject godotObject)
    {
        // Null check, and return nothing false because there's no way to execute the rest of the code
        // if godotObject is null
        if (godotObject == null)
        {
            return false;
        }
        
        // Try to find a CharacterStats object inside the object we hit earlier, by first casting it to a node type
        var characterStats = CraterFunctions.GetNodeByClassFromRoot<CharacterStats>(godotObject as Node);
        
        // Check that CharacterStats isn't null first, because it GetNodeByClassFromRoot can return a null value.
        // Boolean AND/OR are executed in order, so the second condition will only be hit (preventing a null
        // reference exception) if the first condition is TRUE/FALSE respectively
        return characterStats != null && TeamFunctions.TeamMatches(characterStats.characterTeam, _gunTeamFilter);
    }
}