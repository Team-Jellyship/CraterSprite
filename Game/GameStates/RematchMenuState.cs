namespace CraterSprite.Game.GameMode;

public class RematchMenuState : MenuState
{
    public override void ExitState()
    {
        base.ExitState();
        
        foreach (var playerData in GameMode.instance.playerData)
        {
            playerData.ResetScore();
        }
    }
}