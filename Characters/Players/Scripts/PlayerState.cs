using CraterSprite.Game.Match3;
using CraterSprite.Game.GameMode;
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
	
	// Special stuff. This should all be moved to another data class
	[Export] private PackedScene _specialScene;
	[Export] private uint _specialCost = 50;
	[Export] private uint _numMeteors = 5;
	
	public readonly CraterEvent<float, float> onSuperchargeChanged = new();
	
	public readonly Match3Container container = new();

	public int playerIndex { get; private set; }

	public override void _Ready()
	{
		base._Ready();

		match3Spawner = CraterFunctions.FindNodeByClass<Match3Spawner>(GetOwner());
		if (match3Spawner == null)
		{
			GD.PrintErr($"[PlayerState] Player{playerIndex} Could not find valid Match3Spawner.");
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

	public void SetPlayerIndex(int index)
	{
		playerIndex = index;
		GD.Print($"[PlayerState] Assigning player index '{index}' to match3 spawner");
		match3Spawner.playerIndex = index;
	}

	public void AddSuperCharge(float chargeAmount)
	{
		_supercharge += chargeAmount;
		onSuperchargeChanged.Invoke(_supercharge, _maxSupercharge);
		// GD.Print($"[PlayerState] supercharge increased to '{_supercharge}'");
	}

	// I don't want the implementation of specials to be inside here,
	// ideally it should be as data-defined as we can make it, but for testing purposes,
	// this is fine
	public void ExecuteSpecial()
	{
		if (superMoveCharge < _specialCost)
		{
			return;
		}

		superMoveCharge -= _specialCost;
		var rivalCamera = GameMode.instance.GetRivalPlayerData(playerIndex).camera;
		var cameraRect = rivalCamera.GetCameraBounds();
		for (var i = 0; i < _numMeteors; ++i)
		{
			var targetPosition = new Vector2(cameraRect.Position.X + cameraRect.Size.X * GD.Randf() * 0.75f, cameraRect.Position.Y - 128.0f * GD.Randf());
			var meteor = CraterFunctions.CreateInstance<Projectile>(_specialScene, targetPosition);
			meteor.velocity = CraterMath.VectorFromAngle(-75.0f) * (32.0f + 100.0f * GD.Randf());
			meteor.SetOwner(this);
		}
	}
}
