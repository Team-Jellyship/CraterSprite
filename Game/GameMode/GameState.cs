using System.Collections.Generic;
using CraterSprite.Match3;
using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.Game.GameMode;

public class GameState
{
    public virtual void EnterState(GameMode mode) {}

    public virtual void ExitState() {}

    public virtual void NotifyGemDestroyed(GameMode mode, int destroyerPlayerIndex, Vector2 offset) {}
    
    public virtual void SetWinner(GameMode mode, int playerIndex) {}

    public float transitionTime = 0.0f;
    public string stateName;
    public bool canSetWinner = false;
}

public class VersusGameState : GameState
{
    public override void EnterState(GameMode mode)
    {
        SpawnPlayers(mode);
    }

    public override void ExitState()
    {
    }

    public override void NotifyGemDestroyed(GameMode mode, int destroyerPlayerIndex, Vector2 offset)
    {
        mode.GetPlayerState(GameMode.GetRivalIndex(destroyerPlayerIndex))?.match3Spawner.QueueRelativeSpawn(mode.settings.gem, offset);
    }
    
    private void SpawnPlayers(GameMode mode)
    {
        var remainingSpawnLocations = new List<SpawnLocation>(mode.spawnLocations);
        for (var i = 0; i < mode.settings.playerCount; ++i)
        {
            GD.Print($"[GameMode] Spawning player {i}...");
            var chosenSpawnIndex = GD.RandRange(0, remainingSpawnLocations.Count - 1);
            var spawnLocation = remainingSpawnLocations[chosenSpawnIndex];
            var playerInstance = mode.settings.player.Instantiate<Node2D>();
            playerInstance.SetGlobalPosition(spawnLocation.GlobalPosition);
            playerInstance.Name = $"Player{i}";
            spawnLocation.Owner.AddChild(playerInstance);
            remainingSpawnLocations.RemoveAt(chosenSpawnIndex);
			
            mode.players.Add(playerInstance);
            CraterFunctions.GetNodeByClass<PlayerController>(playerInstance)?.BindInput(i);

            mode.onPlayerSpawned.Invoke(i, playerInstance);

            var playerState = CraterFunctions.GetNodeByClass<PlayerState>(playerInstance);
            if (playerState == null)
            {
                continue;
            }
			
            mode.playerStates.Add(playerState);
            var i1 = i;
            playerState.container.onSpawnSingleRequested.AddListener(orb => SpawnEnemyFromSingleType(mode, i1, orb));
            playerState.container.onSpawnRequested.AddListener(orbs => SpawnEnemyFromMatch3(mode, i1, orbs));
            playerState.index = i;
        }
    }
    
    private void SpawnEnemyFromMatch3(GameMode mode, int ownerIndex, List<MatchType> orbElements)
    {
        var spawner = mode.GetPlayerState(GameMode.GetRivalIndex(ownerIndex)).match3Spawner;
        // spawner.QueueSpawn();
    }

    private void SpawnEnemyFromSingleType(GameMode mode, int ownerIndex, MatchType orbType)
    {
        var enemyInstance = mode.recipes.GetEnemyFromOrbType(orbType);
        if (enemyInstance == null)
        {
            return;
        }
        var spawner = mode.GetPlayerState(GameMode.GetRivalIndex(ownerIndex)).match3Spawner;
        spawner.QueueSpawn(enemyInstance);
    }
}