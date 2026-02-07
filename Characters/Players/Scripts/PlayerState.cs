using Godot;

namespace CraterSprite;

public partial class PlayerState : CharacterStats
{
    [Export] public float score;
    [Export] public float superMoveCharge
    {
        get => _superMoveCharge;
        set => _superMoveCharge = Mathf.Clamp(value, 0.0f, _maxSuperMoveCharge);
    }
    private float _superMoveCharge;
    
    [Export] private float _maxSuperMoveCharge;
}