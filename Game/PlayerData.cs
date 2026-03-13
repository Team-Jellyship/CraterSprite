using Godot;

namespace CraterSprite.Game;

public enum RoundOutcome
{
    Win,
    Lose,
    Draw
}

public class PlayerData
{
    public uint playerScore { get; private set; }
    public PlayerState playerState { get; private set; }
    public Node2D player { get; private set; }

    public SubViewportContainer playerViewport;
    
    public PlayerCamera camera;

    public RoundOutcome lastRoundOutcome = RoundOutcome.Win;
    
    public readonly CraterEvent<uint> onScoreChanged = new();



    public void SetPlayer(Node2D playerNode, PlayerState state)
    {
        player = playerNode;
        playerState = state;
        player.TreeExiting += () =>
        { 
            player = null;
            playerState = null;
        };
    }

    public void IncreaseScore()
    {
        onScoreChanged.Invoke(++playerScore);
    }

    public void ResetScore()
    {
        playerScore = 0;
        onScoreChanged.Invoke(0);
    }
}