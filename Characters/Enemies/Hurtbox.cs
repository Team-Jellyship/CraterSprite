using Godot;
using System;

public partial class Hurtbox : Area2D
{
	[Export] private CollisionShape2D _onHitbox;
	[Export] private CollisionShape2D _offHitbox;

	private void SetOff()
	{
		_offHitbox.Disabled = false;
		_onHitbox.Disabled = true;
	}

	private void SetOn()
	{
		_onHitbox.Disabled = false;
		_offHitbox.Disabled = true;
	}
}
