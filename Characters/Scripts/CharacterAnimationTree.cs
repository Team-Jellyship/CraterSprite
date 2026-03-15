using Godot;

namespace CraterSprite;

public partial class CharacterAnimationTree : AnimationTree
{
	private void SetWalkDirection(float direction)
	{
		Set("parameters/state_machine/walking/walk_direction/blend_position", direction);
	}

	private void SetWalkSpeed(float speed)
	{
		Set("parameters/state_machine/walking/walk_speed/scale", speed);
	}
}
