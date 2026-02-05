using Godot;

namespace CraterSprite;

public partial class CharacterAnimationTree : AnimationTree
{
    private void SetWalkSpeed(float speed)
    {
        Set("parameters/walk_direction/blend_position", speed);
    }
}