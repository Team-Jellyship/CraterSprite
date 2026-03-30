using System;
using Godot;

namespace CraterSprite;

public partial class CharacterAnimationTree : AnimationTree
{
	[Export] private float _walkSpeed = 0.0f;
	[Export] private float _walkDirection = 0.0f;
	[Export] private float _aimDirection = -1.0f;
	[Export] private bool _grounded = true;
	private void SetWalkDirection(float direction)
	{
		_walkDirection = direction;
		Set("parameters/state_machine/jumping/blend_position", new Vector2(_walkDirection, _aimDirection));
		Set("parameters/state_machine/walking/walkAimBlend/blend_position", new Vector2(_walkDirection, _aimDirection));
		Set("parameters/state_machine/idle/blend_position", new Vector2(_walkDirection, _aimDirection));
	}

	private void SetWalkSpeed(float speed)
	{
		_walkSpeed = speed;
		Set("parameters/state_machine/walking/walk_speed/scale", speed);
	}

	private void SetAimDirection(AimDirection direction)
	{
		_aimDirection = GetAimDirectionAsFloat(direction);
		Set("parameters/state_machine/jumping/blend_position", new Vector2(_walkDirection, _aimDirection));
		Set("parameters/state_machine/walking/walkAimBlend/blend_position", new Vector2(_walkDirection, _aimDirection));
		Set("parameters/state_machine/idle/blend_position", new Vector2(_walkDirection, _aimDirection));
	}

	private void SetGrounded(bool grounded)
	{
		_grounded = grounded;
	}

	private static float GetAimDirectionAsFloat(AimDirection direction)
	{
		return direction switch
		{
			AimDirection.Horizontal => -1.0f,
			AimDirection.Diagonal => 0.0f,
			AimDirection.Vertical => 1.0f,
			_ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
		};
	}
}
