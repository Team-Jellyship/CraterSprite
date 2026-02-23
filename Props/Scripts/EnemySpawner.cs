using Godot;

namespace CraterSprite.Props;

public partial class EnemySpawner : Area2D
{
    public void SpawnEnemy(PackedScene packedScene)
    {
        CraterFunctions.CreateInstanceDeferred<Node2D>(this, packedScene, GlobalPosition);
    }

    public bool CanSpawn()
    {
        return !HasOverlappingBodies();
    }
}