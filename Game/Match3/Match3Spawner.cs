using System.Collections.Generic;
using CraterSprite.Props;
using Godot;

namespace CraterSprite.Game.Match3;

public partial class Match3Spawner : Node2D
{
    private Queue<PackedScene> _pendingSpawns = [];
    private List<EnemySpawner> _enemySpawners = [];

    public override void _Ready()
    {
        _enemySpawners = CraterFunctions.GetAllNodesByClass<EnemySpawner>(this);
        GD.Print($"[Match3Spawner] Found {_enemySpawners.Count} spawners");
    }

    public void QueueSpawn(PackedScene enemySpawn)
    {
        GD.Print($"[Match3Spawner] Queueing spawn of {enemySpawn.GetPath()}");
        // Try to spawn right away
        var spawner = GetRandomAvailableSpawner();
        if (spawner == null)
        {
            _pendingSpawns.Enqueue(enemySpawn);
            return;
        }
        
        spawner.SpawnEnemy(enemySpawn);
    }

    public void QueueRelativeSpawn(PackedScene spawn, Vector2 offset)
    {
        var newOffset = offset;
        newOffset.X *= -1.0f;
        CraterFunctions.CreateInstanceDeferred<Node2D>(this, spawn, GlobalPosition + newOffset);
    }

    private EnemySpawner GetRandomAvailableSpawner()
    {
        var pendingSpawners = new List<EnemySpawner>(_enemySpawners);

        for (var i = 0; i < _enemySpawners.Count; ++i)
        {
            var chosenSpawner = CraterMath.ChooseRandom(pendingSpawners);
            if (chosenSpawner.CanSpawn())
            {
                return chosenSpawner;
            }
            pendingSpawners.Remove(chosenSpawner);
        }

        return null;
    }
}