using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CraterSprite.Match3;

[GlobalClass]
public partial class Match3Recipe : Resource
{
    public Match3Recipe()
    {
        _recipe = [MatchType.None, MatchType.None, MatchType.None];
    }
    public Match3Recipe(MatchType type1, MatchType type2, MatchType type3)
    {
        _recipe = [type1, type2, type3];
    }
    
    [Export] public MatchType type1
    {
        private set => _recipe[0] = value;
        get => _recipe[0];
    }
    
    [Export] public MatchType type2
    {
        private set => _recipe[1] = value;
        get => _recipe[1];
    }

    [Export] public MatchType type3
    {
        private set => _recipe[2] = value;
        get => _recipe[2];
    }

    private readonly List<MatchType> _recipe = [MatchType.None, MatchType.None, MatchType.None];
    
    public bool CanMake(List<MatchType> ingredients)
    {
        if (ingredients.Count > 3)
        {
            return false;
        }

        var recipeCopy = new List<MatchType>(_recipe);
        return ingredients.All(ingredient => recipeCopy.Remove(ingredient));
    }
}