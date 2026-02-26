using System.Collections.Generic;
using CraterSprite.Props;
using Godot;

namespace CraterSprite.Game.Match3;

public partial class Match3Spawner : Node2D
{
    [Export] public bool enabled = true;
    [Export(PropertyHint.Range, "0,10,0.1f,suffix:x")] private float _spawnMultiplier = 1.0f;
    // How quickly the spawn multiplier increases over time, in units/s
    [Export(PropertyHint.None, "suffix:x/s")] private float _spawnMultiplierIncrease = 0.01f;
    
    [ExportGroup("Enemy Spawns")]
    // Spawn table for enemies only
    [Export] private SpawnTable.SpawnTable _enemySpawnTable;
    
    // How long it takes to spawn a new enemy
    [Export] private float _enemySpawnTime = 10.0f;

    [Export] private uint _maxEnemyCount = 5;
    [Export] private uint _currentEnemyCount = 0;
    
    [ExportGroup("Other spawns")]
    [Export] private SpawnTable.SpawnTable _otherSpawnTable;
    [Export] private float _otherSpawnTime = 15.0f;
    
    // Pending enemy spawns is a separate queue, because we'll register the death callback once the enemies spawn
    private Queue<PackedScene> _pendingEnemySpawns = [];
    private Queue<PackedScene> _pendingSpawns = [];
    
    private float _currentEnemySpawnTime = 0.0f;
    private float _currentOtherSpawnTime = 0.0f;
    
    private List<EnemySpawner> _enemySpawners = [];


    public override void _Ready()
    {
        _enemySpawners = CraterFunctions.GetAllNodesByClass<EnemySpawner>(this);
        GD.Print($"[Match3Spawner] Found {_enemySpawners.Count} spawners");
    }

    public override void _Process(double delta)
    {
        if (!enabled)
        {
            return;
        }
        
        var deltaTime = (float)delta;

        _spawnMultiplier += _spawnMultiplierIncrease * deltaTime;
        UpdateEnemySpawns(deltaTime);
        UpdatePickupSpawns(deltaTime * _spawnMultiplier);
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

    private void QueueEnemySpawn(PackedScene enemySpawn)
    {
        GD.Print($"[Match3Spawner] Queueing spawn of {enemySpawn.GetPath()}");
        // Try to spawn right away
        
        // Enemy count immediately increases, regardless of whether the enemy goes in the queue or not
        ++_currentEnemyCount;
        var spawner = GetRandomAvailableSpawner();
        if (spawner == null)
        {
            _pendingSpawns.Enqueue(enemySpawn);
            return;
        }
        
        var spawnedEnemy = spawner.SpawnEnemy(enemySpawn);
        if (spawnedEnemy == null)
        {
            return;
        }

        spawnedEnemy.TreeExited += () =>
        { if (_currentEnemyCount > 0)
        {
            --_currentEnemyCount;
        } };
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
            if (chosenSpawner != null && chosenSpawner.CanSpawn())
            {
                return chosenSpawner;
            }
            pendingSpawners.Remove(chosenSpawner);
        }

        return null;
    }

    private void UpdateEnemySpawns(float deltaTime)
    {
        if (_currentEnemyCount >= _maxEnemyCount)
        {
            return;
        }
        
        _currentEnemySpawnTime += deltaTime;

        if (_currentEnemySpawnTime < _enemySpawnTime)
        {
            return;
        }
        
        QueueEnemySpawn(_enemySpawnTable.GetRandomEntry());
        _currentEnemySpawnTime -= _enemySpawnTime;
    }

    private void UpdatePickupSpawns(float deltaTime)
    {
        _currentOtherSpawnTime += deltaTime;

        if (_currentOtherSpawnTime < _otherSpawnTime)
        {
            return;
        }
        
        QueueSpawn(_otherSpawnTable.GetRandomEntry());
        _currentOtherSpawnTime -= _otherSpawnTime;
    }
}