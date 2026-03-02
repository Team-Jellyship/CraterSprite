using System;
using System.Collections.Generic;
using CraterSprite.Effects;
using CraterSprite.Match3;
using Godot;

namespace CraterSprite;

/**
 * This class represents the rules of the game
 * for the most part, it shouldn't change once the game is started
 */
public partial class GameMode : Node
{
	public static GameMode instance { get; private set; }
	
	public StatusEffectList statusEffects { get; private set; }
	
	public Match3RecipeTable recipes { get; private set; }

	public CraterEvent<int, Node2D> onPlayerSpawned = new ();
	public Node worldRoot { get; private set; }
	public readonly List<Node2D> players = [];
	
	// Serialized settings, because this is a singleton
	public GameModeSettings settings { get; private set; }
	
	
	public readonly List<SpawnLocation> spawnLocations = [];
	public readonly List<PlayerState> playerStates = [];
	
	private GameState _currentGameState;
	private VersusGameState versusGameState { get; } = new();

	public override void _EnterTree()
	{
		instance = this;
	}

	public override void _Ready()
	{
		statusEffects = ResourceLoader.Load<StatusEffectList>("res://Game/Effects/SL_Effects.tres");
		recipes = ResourceLoader.Load<Match3RecipeTable>("res://Game/Match3/Recipes/M3t_Default.tres");
		ImGuiGodot.ImGuiGD.ToolInit();

		settings = ResourceLoader.Load<GameModeSettings>("res://Game/DefaultSettings.tres");

		_currentGameState = versusGameState;
		versusGameState.EnterState(this);

		worldRoot = spawnLocations[0].Owner;
	}

	public void AddSpawnLocation(SpawnLocation location)
	{
		spawnLocations.Insert((int)location.defaultIndex, location);
	}

	public PlayerState GetPlayerState(int playerIndex)
	{
		if (playerIndex >= 0 && playerIndex < playerStates.Count)
		{
			return playerStates[playerIndex];
		}
		
		GD.PrintErr("[GameMode] Attempted to access a player state from an out of bounds index.");
		return null;
	}

	public void NotifyGemDestroyed(int destroyerPlayerIndex, Vector2 offset)
	{
		_currentGameState.NotifyGemDestroyed(this, destroyerPlayerIndex, offset);
	}

	public static int GetRivalIndex(int playerIndex)
	{
		return playerIndex switch
		{
			0 => 1,
			1 => 0,
			_ => throw new NotImplementedException()
		};
	}
}
