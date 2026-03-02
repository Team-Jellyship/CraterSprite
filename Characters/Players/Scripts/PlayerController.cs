using Godot;
using System;
using CraterSprite.Input;

namespace CraterSprite
{
	public partial class PlayerController : Node
	{
		[Export] private KinematicCharacter _character;
		[Export] private ProjectileLauncher _gun;
		
		public void BindInput(int deviceIndex)
		{
			InputManager.instance.RegisterCallback("walk", InputEventType.Changed, strength =>
			{
				_character.SetMoveInput(strength);
			}, deviceIndex, this);
			
			InputManager.instance.RegisterCallback("jump", InputEventType.Pressed, _ => _character.StartJumping(), deviceIndex, this);
			InputManager.instance.RegisterCallback("jump", InputEventType.Released, _ => _character.StopJumping(), deviceIndex,this);
			
			InputManager.instance.RegisterCallback("crouch", InputEventType.Pressed, _ => _character.Crouch(), deviceIndex,this);
			InputManager.instance.RegisterCallback("crouch", InputEventType.Released, _ => _character.Uncrouch(), deviceIndex,this);
			
			InputManager.instance.RegisterCallback("fire", InputEventType.Pressed, _ => _gun.FireProjectile(), deviceIndex, this);
			InputManager.instance.RegisterCallback("aim_up", InputEventType.Pressed, _ => _gun.aimingUp = true, deviceIndex, this);
			InputManager.instance.RegisterCallback("aim_up", InputEventType.Released, _ => _gun.aimingUp = false, deviceIndex, this);
			
			InputManager.instance.RegisterCallback("input_debug_toggle", InputEventType.Pressed, _ => InputManager.instance.ToggleDebug(), deviceIndex, this);
		}
	}
}
