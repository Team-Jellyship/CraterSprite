namespace CraterSprite.Game.GameMode;

public class LoadingGameState : GameState
{
    public override void EnterState(GameMode mode)
    {
        // Display loading screen here
        mode.nextLevel = mode.settings.defaultLevel;
        mode.LoadLevel();
        mode.Command(GameModeCommand.Loaded);
    }
}