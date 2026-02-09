using Godot;
using System;
using CraterSprite.Input;

namespace CraterSprite
{
    public partial class PlayerController : Node
    {
        [Export] private KinematicCharacter _character;
        [Export] private ProjectileLauncher _gun;

        [Export] private int _controllerId = 0;
        
        public override void _Ready()
        {
            InputManager.instance.RegisterCallback("walk", InputEventType.Changed, strength =>
            {
                _character.SetMoveInput(strength);
            }, _controllerId, this);
            
            InputManager.instance.RegisterCallback("jump", InputEventType.Pressed, _ => _character.StartJumping(), _controllerId, this);
            InputManager.instance.RegisterCallback("jump", InputEventType.Released, _ => _character.StopJumping(), _controllerId,this);
            
            InputManager.instance.RegisterCallback("crouch", InputEventType.Pressed, _ => _character.Crouch(), _controllerId,this);
            InputManager.instance.RegisterCallback("crouch", InputEventType.Released, _ => _character.Uncrouch(), _controllerId,this);
            
            InputManager.instance.RegisterCallback("fire", InputEventType.Pressed, _ => _gun.FireProjectile(), _controllerId, this);
            InputManager.instance.RegisterCallback("aim_up", InputEventType.Pressed, _ => _gun.aimingUp = true, _controllerId, this);
            InputManager.instance.RegisterCallback("aim_up", InputEventType.Released, _ => _gun.aimingUp = false, _controllerId, this);
        }
    }
}
