using Godot;
using CraterSprite;
using CraterSprite.Shared.Scripts;

public partial class Projectile : Node2D
{
    [Export] private bool _destroyOnContact = true;
    [Export] private float _lifetime = 1.0f;
    [Export] private bool _collectGems = true;
    
    [Signal] public delegate void OnHitEventHandler();

    private EncodedObjectAsId _owner;
    public Vector2 velocity;

    public override void _EnterTree()
    {
        var expirationTimer = new Timer();
        expirationTimer.Autostart = true;
        expirationTimer.OneShot = true;
        expirationTimer.WaitTime = _lifetime;
        AddChild(expirationTimer);
        expirationTimer.Timeout += () =>
        {
            EmitSignalOnHit();
            QueueFree();
        };

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
        var hitObjectCharacterStats = CraterFunctions.GetNodeByClassFromParent<IDamageListener>(area);
        switch (hitObjectCharacterStats)
        {
            case null:
            case Node node when node.GetInstanceId() == _owner.ObjectId:
                return;

            default:
                HitEnemy(hitObjectCharacterStats);
                break;
        }

    }

    private void HitEnemy(IDamageListener character)
    {
        var obj = InstanceFromId(_owner.ObjectId);
        // If this shouldn't collect gems, just don't give it the damage source
        // and it won't receive the event for now
        character.TakeDamage(1.0f, _collectGems ? (CharacterStats)obj : null, false);

        EmitSignalOnHit();
        
        if (_destroyOnContact)
        {
            QueueFree();
        }
    }
}
