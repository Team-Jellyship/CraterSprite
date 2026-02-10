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

    private List<SpawnLocation> _locations = [];
    
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
    }

    public void AddSpawnLocation(SpawnLocation location)
    {
        GD.Print("Loading spawn location");
        _locations.Insert((int)location.defaultIndex, location);
    }

    private void SpawnPlayers()
    {
        var remainingSpawnLocations = new List<SpawnLocation>(_locations);
        for (var i = 0; i < _settings.playerCount; ++i)
        {
            GD.Print($"[GameMode] Spawning player {i}...");
            var chosenSpawnIndex = GD.RandRange(0, remainingSpawnLocations.Count - 1);
            CraterFunctions.CreateInstanceDeferred<Node2D>(this, _settings.player, remainingSpawnLocations[chosenSpawnIndex].GlobalPosition);
            remainingSpawnLocations.RemoveAt(chosenSpawnIndex);
        }
    }
}