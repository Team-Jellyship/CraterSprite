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

    
    // ==== Player objects ====
    // Possible spawn locations
    private readonly List<SpawnLocation> _locations = [];
    
    // Current player objects
    public readonly List<Node2D> players = [];
    
    // Current player state objects. These are where the relevant attributes
    // and other things that persist beyond the player's death go
    private readonly List<WeakReference<PlayerState>> _playerStates = [];
    
    // Because of the weird split-screen setup, objects should refer to this
    // when they are spawning into the world, rather than using the current
    // tree root
    public Node worldRoot { get; private set; }

    // Event for when a player is spawned. UI objects might need to listen for this
    public readonly CraterEvent<int, Node2D> onPlayerSpawned = new ();
    
    // Serialized settings, because this is a singleton
    private GameModeSettings _settings;

    public override void _EnterTree()
    {
        instance = this;
    }

    public override void _Ready()
    {
        statusEffects = ResourceLoader.Load<StatusEffectList>("res://Game/Effects/SL_Effects.tres");
        recipes = ResourceLoader.Load<Match3RecipeTable>("res://Game/Match3/Recipes/M3t_Default.tres");
        ImGuiGodot.ImGuiGD.ToolInit();
        
        var ingredients0 = new List<MatchType> { MatchType.Fire, MatchType.Fire };
        var ingredients1 = new List<MatchType> { MatchType.Fire, MatchType.Water };
        var ingredients2 = new List<MatchType> { MatchType.Fire, MatchType.Fire, MatchType.Fire };
        var ingredients3 = new List<MatchType> { MatchType.Fire, MatchType.Fire, MatchType.Fire, MatchType.Fire };

        GD.Print($"Loaded {recipes.count} recipes.");
        
        GD.Print($"Make recipe '{string.Join(", ", ingredients0)}': {recipes.GetEnemy(ingredients0)}");
        GD.Print($"Make recipe '{string.Join(", ", ingredients1)}': {recipes.GetEnemy(ingredients1)}");
        GD.Print($"Make recipe '{string.Join(", ", ingredients2)}': {recipes.GetEnemy(ingredients2)}");
        GD.Print($"Make recipe '{string.Join(", ", ingredients3)}': {recipes.GetEnemy(ingredients3)}");

        _settings = ResourceLoader.Load<GameModeSettings>("res://Game/DefaultSettings.tres");
        SpawnPlayers();

        worldRoot = _locations[0].Owner;
    }

    /**
     * <summary>Register a new potential spawn location for the players</summary>
     */
    public void AddSpawnLocation(SpawnLocation location)
    {
        _locations.Insert((int)location.defaultIndex, location);
    }

    /**
     * <summary>Get the player state associated with a specific player index</summary>
     * <returns>Player state, or null if there is no associated index</returns>
     */
    public PlayerState GetPlayerState(int playerIndex)
    {
        if (playerIndex >= 0 && playerIndex < _playerStates.Count)
        {
            _playerStates[playerIndex].TryGetTarget(out var playerState);
            return playerState;
        }
        
        GD.PrintErr("[GameMode] Attempted to access a player state from an out of bounds index.");
        return null;
    }

    public void NotifyGemDestroyed(int destroyerPlayerIndex, Vector2 offset)
    { 
        GetPlayerState(GetRivalIndex(destroyerPlayerIndex))?.match3Spawner.QueueRelativeSpawn(_settings.gem, offset);
    }

    private void SpawnPlayers()
    {
        var remainingSpawnLocations = new List<SpawnLocation>(_locations);
        for (var i = 0; i < _settings.playerCount; ++i)
        {
            GD.Print($"[GameMode] Spawning player {i}...");
            var chosenSpawnIndex = GD.RandRange(0, remainingSpawnLocations.Count - 1);
            var spawnLocation = remainingSpawnLocations[chosenSpawnIndex];
            var playerInstance = _settings.player.Instantiate<Node2D>();
            playerInstance.SetGlobalPosition(spawnLocation.GlobalPosition);
            playerInstance.Name = $"Player{i}";
            spawnLocation.Owner.AddChild(playerInstance);
            remainingSpawnLocations.RemoveAt(chosenSpawnIndex);
            
            players.Add(playerInstance);
            CraterFunctions.GetNodeByClass<PlayerController>(playerInstance)?.BindInput(i);

            onPlayerSpawned.Invoke(i, playerInstance);

            var playerState = CraterFunctions.GetNodeByClass<PlayerState>(playerInstance);
            if (playerState == null)
            {
                continue;
            }
            
            _playerStates.Add(new WeakReference<PlayerState>(playerState));
            var i1 = i;
            playerState.container.onSpawnSingleRequested.AddListener(orb => SpawnEnemyFromSingleType(i1, orb));
            playerState.container.onSpawnRequested.AddListener(orbs => SpawnEnemyFromMatch3(i1, orbs));
            playerState.index = i;
        }
    }

    private void SpawnEnemyFromMatch3(int ownerIndex, List<MatchType> orbElements)
    {
        var spawner = GetPlayerState(GetRivalIndex(ownerIndex)).match3Spawner;
        // spawner.QueueSpawn();
    }

    private void SpawnEnemyFromSingleType(int ownerIndex, MatchType orbType)
    {
        var enemyInstance = recipes.GetEnemyFromOrbType(orbType);
        if (enemyInstance == null)
        {
            return;
        }
        
        GetPlayerState(GetRivalIndex(ownerIndex))?.match3Spawner.QueueSpawn(enemyInstance);
    }

    private static int GetRivalIndex(int playerIndex)
    {
        return playerIndex switch
        {
            0 => 1,
            1 => 0,
            _ => throw new NotImplementedException()
        };
    }
}