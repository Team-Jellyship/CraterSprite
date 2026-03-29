using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.Props;

public partial class SimpleParticleSpawner : Node2D
{
    [Export] private SpriteFrames _particleSprite;
    [Export] private Vector2 _offset;
    [Export] private bool _flipH;
    [Export] private bool _flipV;

    private void Spawn()
    {
        var animatedSprite = new AnimatedSprite2D();
        GameMode.instance.worldRoot.AddChild(animatedSprite);
        animatedSprite.GlobalPosition = GlobalPosition + GetFlippedOffset();
        animatedSprite.FlipH = _flipH;
        animatedSprite.FlipV = _flipV;
        animatedSprite.SpriteFrames = _particleSprite;
        animatedSprite.Play("default");
        animatedSprite.AnimationFinished += animatedSprite.QueueFree;
    }

    // Also flip the offset
    private Vector2 GetFlippedOffset()
    {
        var result = _offset;
        if (_flipH)
        {
            result.X = -result.X;
        }

        if (_flipV)
        {
            result.Y = -result.Y;
        }
        return result;
    }

    private void DirectionChanged(float direction)
    {
        _flipH = direction < 0.0f;
        Spawn();
    }
}