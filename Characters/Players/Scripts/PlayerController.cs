using Godot;
using System;

namespace CraterSprite
{
    public partial class PlayerController : Node
    {
        [Export] private KinematicCharacter _character;
        
        public override void _Ready()
        {
            InputManager.instance.RegisterCallback("walk_left", strength =>
            {
                _character.SetMoveInput(strength);
            }, this);
        }
    }
}
