using System.Collections.Generic;
using CraterSprite.Match3;
using Godot;

namespace CraterSprite.Game.GameMode;

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
			
            mode.onPlayerSpawned.Invoke(i, playerInstance);
            CraterFunctions.GetNodeByClass<PlayerController>(playerInstance)?.BindInput(i);


            var playerState = CraterFunctions.GetNodeByClass<PlayerState>(playerInstance);
            if (playerState == null)
            {
                GD.PrintErr($"[GameState] Could not find player state for Player{i}");
                continue;
            }
			
            var i1 = i;
            playerState.container.onSpawnSingleRequested.AddListener(orb => SpawnEnemyFromSingleType(mode, i1, orb));
            playerState.container.onSpawnRequested.AddListener(orbs => SpawnEnemyFromMatch3(mode, i1, orbs));
            playerState.SetPlayerIndex(i);
            GD.Print($"[GameState] Successfully spawned Player{i}");
            playerState.OnDeath += () =>
            {
                mode.SetWinner(GameMode.GetRivalIndex(i1));
                mode.Command(GameModeCommand.Victory);
            };
            
            mode.playerData[i].SetPlayer(playerInstance, playerState);

            var playerAnimator = CraterFunctions.GetNodeByClass<AnimatedSprite2D>(playerInstance);
            if (playerAnimator == null)
            {
                return;
            }
            playerAnimator.SpriteFrames = mode.playerData[i].playerSpriteFrames;
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

        var index = GameMode.GetRivalIndex(ownerIndex);
        GD.Print($"[GameState] Requesting spawn of enemy '{enemyInstance.GetPath()} for Player{index}");
        var playerState = mode.GetPlayerState(index);
        if (playerState == null)
        {
            GD.PrintErr($"[GameState] Failed so spawn enemy. Could not find player state for Player{index}");
            return;
        }
        
        var spawner = playerState.match3Spawner;
        if (spawner == null)
        {
            GD.PrintErr($"[GameState] Failed to spawn enemy. Could not find spawner for Player{index}");
            return;
        }
        spawner.QueueSpawn(enemyInstance);
    }
}