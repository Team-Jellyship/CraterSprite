using CraterSprite.Game.Match3;
using Godot;

namespace CraterSprite;

public partial class PlayerState : CharacterStats
{
	[Export] public float score;
	[Export] public float superMoveCharge
	{
		get => _supercharge;
		set => _supercharge = Mathf.Clamp(value, 0.0f, _maxSupercharge);
	}
	private float _supercharge;
	
	[Export] private float _maxSupercharge;
	[Export] public Match3Spawner match3Spawner { private set; get; }
	
	
	public readonly CraterEvent<float, float> onSuperchargeChanged = new();
	
	public readonly Match3Container container = new();

	public int index;

	public override void _Ready()
	{
		base._Ready();

		match3Spawner = CraterFunctions.FindNodeByClass<Match3Spawner>(GetOwner());
		if (match3Spawner == null)
		{
			GD.PrintErr("[PlayerState] Could not find valid Match3Spawner.");
		}
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		
		AddSuperCharge((float)delta * 1.0f);
	}

	public override void KilledEnemy(CharacterStats enemy)
	{
		container.AddOrb(enemy.matchType);
	}

	public void AddSuperCharge(float chargeAmount)
	{
		_supercharge += chargeAmount;
		onSuperchargeChanged.Invoke(_supercharge, _maxSupercharge);
		// GD.Print($"[PlayerState] supercharge increased to '{_supercharge}'");
	}
}
