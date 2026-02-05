using Godot;
using System;

namespace CraterSprite
{
    public partial class PlayerController : Node
    {
        [Export] private KinematicCharacter _character;
        
        public override void _Ready()
        {
            InputManager.instance.RegisterAxisChangedCallback("walk_right", "walk_left", strength =>
            {
                _character.SetMoveInput(strength);
            }, this);
            
            InputManager.instance.RegisterCallback("jump", InputEventType.Pressed, _ => _character.StartJumping(), this);
            InputManager.instance.RegisterCallback("jump", InputEventType.Released, _ => _character.StopJumping(), this);
            
            InputManager.instance.RegisterCallback("crouch", InputEventType.Pressed, _ => _character.Crouch(), this);
            InputManager.instance.RegisterCallback("crouch", InputEventType.Released, _ => _character.Uncrouch(), this);
        }
    }
}
