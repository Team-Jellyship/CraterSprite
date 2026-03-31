using System;
using System.Collections.Generic;
using CraterSprite.Effects;
using CraterSprite.Input;
using CraterSprite.Match3;
using Godot;

namespace CraterSprite.Game.GameMode;

/**
 * This class represents the rules of the game
 * for the most part, it shouldn't change once the game is started
 */
public partial class GameMode : Node
{
	public static GameMode instance { get; private set; }
	public StatusEffectList statusEffects { get; private set; }
	public Match3RecipeTable recipes { get; private set; }
	public Node worldRoot { get; private set; }
	public Node2D sharedRoot { get; private set; }
	public Control menuRoot { get; private set; }
	
	// Serialized settings, because this is a singleton
	public GameModeSettings settings { get; private set; }

	public readonly List<PlayerData> playerData = [new(), new()];
	public readonly List<SpawnLocation> spawnLocations = [];
	public PackedScene nextLevel;
	
	public readonly CraterEvent<int, Node2D> onPlayerSpawned = new ();
	
	public bool showingDebug { private set; get; }
	
	private Timer _transitionTimer = new();
	private Node _sceneEntryPoint;

	
	// Game States
	private GameState _currentGameState;

	private LoadingGameState loadingState { get; } = new()
	{
		stateName = "Loading"
	};

	private MenuState characterSelectState { get; } = new()
	{
		stateName = "Character Select",
		transitionTime = 3.0f
	};

	private RematchMenuState rematchState { get; } = new()
	{
		stateName = "GameOver"
	};
	private VersusGameState versusGameState { get; } = new()
	{
		stateName = "Versus",
		canSetWinner = true
	};
	private RoundOverState roundOverState { get; } = new()
	{
		stateName = "Victory",
		transitionTime = 2.0f
	};
	
	// Transition table
	private readonly Dictionary<Tuple<GameState, GameModeCommand>, Func<GameState>> _transitions = new();


	public override void _EnterTree()
	{
		instance = this;
	}

	public override void _Ready()
	{
		ImGuiGodot.ImGuiGD.ToolInit();
		ProcessMode = ProcessModeEnum.Always;
		
		LoadSettings();
		statusEffects = ResourceLoader.Load<StatusEffectList>("res://Game/Effects/SL_Effects.tres");
		recipes = ResourceLoader.Load<Match3RecipeTable>("res://Game/Match3/M3t_RecipeTable.tres");

		var currentScene = GetTree().GetCurrentScene();
		_sceneEntryPoint = currentScene.GetNode("%WorldRoot");
		sharedRoot = currentScene.GetNode<Node2D>("%SharedRoot");
		menuRoot = currentScene.GetNode<Control>("%MenuRoot");

		if (_sceneEntryPoint == null)
		{
			GD.PrintErr("[GameMode] Could not find a valid world root node.");
			return;
		}

		_transitionTimer = CraterFunctions.CreateTimer(this, "TransitionTimer", () => Command(GameModeCommand.Timeout));
		characterSelectState.menuScene = settings.characterSelectScreen;
		rematchState.menuScene = settings.rematchScreen;

		var roundStartState = new RoundStartState
		{
			stateName = "RoundStart",
			menuScene = settings.roundIntroScreen,
			transitionTime = 2.0f
		};
		
		//Transition Handling
		_transitions.Add(new Tuple<GameState, GameModeCommand>(characterSelectState, GameModeCommand.Victory), () => roundStartState);
		_transitions.Add(new Tuple<GameState, GameModeCommand>(roundStartState, GameModeCommand.Timeout), () => loadingState);
		_transitions.Add(new Tuple<GameState, GameModeCommand>(loadingState, GameModeCommand.Loaded), () => versusGameState);
		_transitions.Add(new Tuple<GameState, GameModeCommand>(versusGameState, GameModeCommand.Victory), () => roundOverState);
		_transitions.Add(new Tuple<GameState, GameModeCommand>(roundOverState, GameModeCommand.Timeout), () =>
		{ return playerData.TrueForAll(data => data.playerScore < settings.roundsToWin) ? loadingState : rematchState; });
		_transitions.Add(new Tuple<GameState, GameModeCommand>(rematchState, GameModeCommand.Victory), () => roundStartState);
		
		_currentGameState = loadingState;
		_currentGameState.EnterState(this);
		if (_currentGameState.transitionTime <= 0.0f)
		{
			_transitionTimer.Stop();
		}
		else
		{
			_transitionTimer.Start(_currentGameState.transitionTime);
		}

		for (var i = 0; i < settings.playerCount; ++i)
		{
			playerData[i].playerViewport = currentScene.GetNode<SubViewportContainer>($"%Viewport{i}");
		}
		
		// Register our debug toggle input listener
		InputManager.instance.RegisterCallback("game_debug_toggle", InputEventType.Pressed, _ =>
		{ 
			showingDebug = !showingDebug;
			GD.Print($"[GameMode] set show game debug to {showingDebug}");
		}, 0, this);
	}

	/**
	 * <summary>Register a new potential spawn location for the players</summary>
	 */
	public void AddSpawnLocation(SpawnLocation location)
	{
		spawnLocations.Insert((int)location.defaultIndex, location);
	}

	/**
	 * <summary>Get the player state associated with a specific player index</summary>
	 * <returns>Player state, or null if there is no associated index</returns>
	 */
	public PlayerState GetPlayerState(int playerIndex)
	{
		return GetPlayerData(playerIndex)?.playerState;
	}

	public PlayerData GetPlayerData(int playerIndex)
	{
		if (playerIndex >= 0 && playerIndex < playerData.Count)
		{
			return playerData[playerIndex];
		}
		
		GD.PrintErr("[GameMode] Attempted to access player data from an out of bounds index.");
		return null;
	}

	// Just a convenience method for players
	public PlayerData GetRivalPlayerData(int selfPlayerIndex)
	{
		return GetPlayerData(GetRivalIndex(selfPlayerIndex));
	}

	public void NotifyGemDestroyed(int destroyerPlayerIndex, Vector2 offset)
	{
		_currentGameState.NotifyGemDestroyed(this, destroyerPlayerIndex, offset);
	}

	public void Command(GameModeCommand command)
	{
		if (!_transitions.TryGetValue(new Tuple<GameState, GameModeCommand>(_currentGameState, command), out var newStateFunction))
		{
			return;
		}

		var newState = newStateFunction.Invoke();
		GD.Print($"[GameMode] Transitioning to new game state '{newState.stateName}'");
		_currentGameState.ExitState();
		_currentGameState = newState;
		_currentGameState.EnterState(this);

		if (_currentGameState.transitionTime <= 0.0f)
		{
			_transitionTimer.Stop();
		}
		else
		{
			GD.Print($"[GameMode] Transition in {_currentGameState.transitionTime}");
			_transitionTimer.Start(_currentGameState.transitionTime);
		}
	}

	public void SetWinner(int playerIndex)
	{
		if (!_currentGameState.canSetWinner)
		{
			return;
		}

		GD.Print($"[GameMode] Player {playerIndex} won!");
		playerData[playerIndex].IncreaseScore();

		for (var i = 0; i < playerData.Count; ++i)
		{
			playerData[i].lastRoundOutcome = i == playerIndex ? RoundOutcome.Win : RoundOutcome.Lose;
		}
		Command(GameModeCommand.Victory);
	}

	public void LoadLevel()
	{
		UnloadLevel();
		LoadLevel(nextLevel);
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

	// Cleanup level contextual objects
	private void UnloadLevel()
	{
		foreach (var node in sharedRoot.GetChildren())
		{
			node.Free();
		}
		
		worldRoot?.Free();
		spawnLocations.Clear();
	}

	/**
	 * <summary>Load a level, and place it correctly in the splitscreen world</summary>
	 */
	private void LoadLevel(PackedScene levelScene)
	{
		worldRoot = levelScene.Instantiate();
		_sceneEntryPoint.AddChild(worldRoot);
	}

	private void LoadSettings()
	{
		settings = ResourceLoader.Load<GameModeSettings>("res://Game/DefaultSettings.tres");
		for (var i = 0; i < playerData.Count && i < settings.playerDefaultSpriteFrames.Count; ++i)
		{
			playerData[i].playerSpriteFrames = settings.playerDefaultSpriteFrames[i];
		}
	}
}
