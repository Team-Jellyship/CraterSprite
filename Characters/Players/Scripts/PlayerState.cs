using CraterSprite.Game.Match3;
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

    [Export] public Match3Spawner match3Spawner { private set; get; }

    public CraterEvent<float> onSuperChargeChanged = new();
    
    public readonly Match3Container container = new();

    public override void _Ready()
    {
        base._Ready();

        match3Spawner = CraterFunctions.FindNodeByClass<Match3Spawner>(GetOwner());
        if (match3Spawner == null)
        {
            GD.PrintErr("[PlayerState] Could not find valid Match3Spawner.");
        }
    }

    public override void KilledEnemy(CharacterStats enemy)
    {
        container.AddOrb(enemy.matchType);
    }

    public void AddSuperCharge(float chargeAmount)
    {
        _superMoveCharge += chargeAmount;
        onSuperChargeChanged.Invoke(_superMoveCharge);
        GD.Print($"[PlayerState] supercharge increased to '{_superMoveCharge}'");
    }
}