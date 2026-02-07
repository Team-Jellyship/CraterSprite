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

        /*
        var recipe = new Match3Recipe(MatchType.Fire, MatchType.Fire, MatchType.Fire);
        var ingredients0 = new List<MatchType> { MatchType.Fire, MatchType.Fire };
        var ingredients1 = new List<MatchType> { MatchType.Fire, MatchType.Water };

        if (recipe.CanMake(ingredients0))
        {
            GD.Print("Can make recipe 0!");
        }
        if (recipe.CanMake(ingredients1))
        {
            GD.Print("Can make recipe 1!");
        }*/
    }
}