using Godot;

namespace CraterSprite.SpawnTable;

[GlobalClass]
public partial class SpawnEntry : Resource 
{
    [Export] public PackedScene prefab;
    [Export] public float spawnChance;
}