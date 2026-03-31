using Godot;
using System;
using CraterSprite.Input;

namespace CraterSprite
{
	public partial class MenuController : Node
	{
		[Export] private MenuControl _controller;
		
		public void BindInput(int deviceIndex)
		{
			/*
			InputManager.instance.RegisterCallback("aim_up", InputEventType.Pressed, _ => _controller.up() = true, deviceIndex, this);
			InputManager.instance.RegisterCallback("crouch", InputEventType.Pressed, _ => _controller.down(), deviceIndex,this);
			
			InputManager.instance.RegisterCallback("jump", InputEventType.Released, _ => _controller.select(), deviceIndex,this);
			InputManager.instance.RegisterCallback("fire", InputEventType.Released, _ => _controller.back(), deviceIndex, this);
			InputManager.instance.RegisterCallback("special", InputEventType.Released, _ => _controller.credits(), deviceIndex, this);
			
			InputManager.instance.RegisterCallback("startP1", InputEventType.Pressed, _ => _controller.StartP1(), 0, this);
			InputManager.instance.RegisterCallback("startP2", InputEventType.Pressed, _ => _controller.StartP2(), 1, this);
			
			InputManager.instance.RegisterCallback("input_debug_toggle", InputEventType.Pressed, _ => InputManager.instance.ToggleDebug(), deviceIndex, this);
			*/
		}
	}
}
