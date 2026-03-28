namespace CraterSprite.Game.GameMode;

public class RoundStartState : MenuState
{
    public override void EnterState(GameMode mode)
    {
        base.EnterState(mode);
        
        SoundtrackPlayer.instance.StartRandomSong();
    }
}