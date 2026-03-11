using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CraterSprite.SpawnTable;

[GlobalClass]
public partial class SpawnTable : Resource
{
    [Export]
    private Godot.Collections.Array<SpawnEntry> entries
    {
        set => _entries = CraterFunctions.ConvertArray(value);
        get => CraterFunctions.ConvertToGodotArray(_entries);
    }
    
    private List<SpawnEntry> _entries = [];

    public PackedScene GetRandomEntry()
    {
        var totalWeight = _entries.Sum(entry => entry.spawnChance);
        var initialWeight = GD.Randf() * totalWeight;
        var currentWeight = initialWeight;

        foreach (var entry in _entries)
        {
            if (currentWeight < entry.spawnChance)
            {
                return entry.prefab;
            }
            currentWeight -= entry.spawnChance;
        }

        return null;
    }
}