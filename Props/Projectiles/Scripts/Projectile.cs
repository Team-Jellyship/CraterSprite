using Godot;
using System;
using CraterSprite;

public partial class Projectile : Node2D
{
    [Export] private bool _destroyOnContact = true;
    [Export] private float _lifetime = 1.0f;
    
    private EncodedObjectAsId _owner;
    public Vector2 velocity;

    public override void _EnterTree()
    {
        var expirationTimer = new Timer();
        expirationTimer.Autostart = true;
        expirationTimer.OneShot = true;
        AddChild(expirationTimer);
        expirationTimer.Timeout += QueueFree;

        var hitbox = CraterFunctions.GetNodeByClass<Area2D>(this);
        if (hitbox != null)
        {
            hitbox.AreaEntered += Overlap;
        }
    }

    public override void _Process(double delta)
    {
        Position += (float)delta * velocity;
    }

    public void SetOwner(CharacterStats owner)
    {
        _owner = new EncodedObjectAsId
        {
            ObjectId = owner.GetInstanceId()
        };
    }

    private void Overlap(Area2D area)
    {
        var hitObjectCharacterStats = CraterFunctions.GetNodeByClassFromRoot<CharacterStats>(area);
        if (hitObjectCharacterStats == null)
        {
            return;
        }
        
        if (hitObjectCharacterStats.GetInstanceId() == _owner.ObjectId)
        {
            return;
        }
        HitEnemy(hitObjectCharacterStats);
    }

    private void HitEnemy(CharacterStats character)
    {
        character.TakeDamage(1.0f);

        if (_destroyOnContact)
        {
            QueueFree();
        }
    }
}
