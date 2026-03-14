using System.Collections.Generic;
using Godot;

namespace CraterSprite.Game.GameMode;

public class RoundOverState : GameState
{
    private readonly List<Node> _spawnedScreens = new();
    public override void EnterState(GameMode mode)
    {
        foreach (var playerData in mode.playerData)
        {
            var resultScene = playerData.lastRoundOutcome == RoundOutcome.Win ? mode.settings.victoryScreen : mode.settings.lossScreen;
            var resultScreenInstance = resultScene.Instantiate();
            playerData.playerViewport.AddChild(resultScreenInstance);
            _spawnedScreens.Add(resultScreenInstance);
        }
    }

    public override void ExitState()
    {
        foreach (var resultScreenInstance in _spawnedScreens)
        {
            resultScreenInstance.Free();
        }
        _spawnedScreens.Clear();
    }
}