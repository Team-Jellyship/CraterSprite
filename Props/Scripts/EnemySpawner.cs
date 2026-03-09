using Godot;

namespace CraterSprite.Props;

public partial class EnemySpawner : Area2D
{
    public Node2D SpawnEnemy(PackedScene packedScene)
    {
        return CraterFunctions.CreateInstanceDeferred<Node2D>(this, packedScene, GlobalPosition);
    }

    public bool CanSpawn()
    {
        return !HasOverlappingBodies();
    }
}