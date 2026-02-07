using System.Collections.Generic;
using CraterSprite.Effects;
using CraterSprite.Match3;
using Godot;

namespace CraterSprite;

public partial class GameMode : Node
{
    public static GameMode instance { get; private set; }
    
    public StatusEffectList statusEffects { get; private set; }
    
    public Match3RecipeTable recipes { get; private set; }

    public override void _Ready()
    {
        instance = this;

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
    }
}