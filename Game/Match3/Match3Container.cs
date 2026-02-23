using System.Collections.Generic;
using CraterSprite.Match3;
using Godot;

namespace CraterSprite;

public class Match3Container
{
    public readonly List<MatchType> orbs = [];

    public readonly CraterEvent<List<MatchType>> onOrbsChanged = [];

    public readonly CraterEvent<MatchType> onSpawnSingleRequested = [];
    public readonly CraterEvent<List<MatchType>> onSpawnRequested = [];

    public void AddOrb(MatchType matchType)
    {
        // Container shouldn't hold the none type
        if (matchType == MatchType.None)
        {
            return;
        }
        
        orbs.Add(matchType);
        OrbsChanged();
    }

    private void OrbsFull()
    {
        
    }

    private void OrbsChanged()
    {
        if (!GameMode.instance.recipes.CanMake(orbs))
        {
            // Notify spawn manager
            GD.Print("Could not spawn anything with this combination.");

            for (var i = 0; i < orbs.Count - 1; ++i)
            {
                onSpawnSingleRequested.Invoke(orbs[i]);
            }
            orbs.RemoveRange(0, orbs.Count - 1);
        }
        
        onOrbsChanged.Invoke(orbs);
    }
}