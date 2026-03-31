using Godot;
using System;

namespace CraterSprite.Characters.Enemies;
    
public partial class WalkerAnimationTree : AnimationTree
{
    [Export] private float _walkSpeed = 0.0f;
    [Export] private float _walkDirection = 0.0f;

    private void SetWalkDirection(float direction)
    {
        _walkDirection = direction;
        Set("parameters/StateMachine/Walk/blend_position", direction);
        Set("parameters/StateMachine/Shoot/blend_position", direction);
    }

    private void Shoot()
    {
        var stateMachine = (AnimationNodeStateMachinePlayback)Get("parameters/StateMachine/playback");
        stateMachine.Travel("Shoot");
    }
}
