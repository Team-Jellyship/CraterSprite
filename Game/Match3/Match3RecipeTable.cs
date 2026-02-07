using System.Collections.Generic;
using Godot;

namespace CraterSprite.Match3;

[GlobalClass]
public partial class Match3RecipeTable : Resource
{
    [Export]
    public Godot.Collections.Array<Match3Recipe> recipes
    {
        set => _recipes = CraterFunctions.ConvertArray(value);
        get => CraterFunctions.ConvertToGodotArray(_recipes);
    }

    private List<Match3Recipe> _recipes = [];
}