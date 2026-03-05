using Godot;

namespace CraterSprite.Game;

public class PlayerData
{
    public uint playerScore { get; private set; }
    public PlayerState playerState { get; private set; }
    public Node2D player { get; private set; }

    public CraterEvent<uint> onScoreChanged = new();

    public void SetPlayer(Node2D playerNode, PlayerState state)
    {
        player = playerNode;
        playerState = state;
        player.TreeExited += () =>
        { 
            player = null;
            playerState = null;
        };
    }

    public void IncreaseScore()
    {
        onScoreChanged.Invoke(++playerScore);
    }
}