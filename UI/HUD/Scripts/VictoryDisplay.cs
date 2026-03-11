using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.UI.HUD.Scripts;

public partial class VictoryDisplay : Control
{
    [Export] private int _playerIndex;
    [Export] private Label _label;

    public override void _Ready()
    {
        GameMode.instance.GetPlayerData(_playerIndex).onScoreChanged.AddListener(ScoreChanged);
    }

    public override void _ExitTree()
    {
        GameMode.instance.GetPlayerData(_playerIndex).onScoreChanged.RemoveListener(ScoreChanged);
    }

    private void ScoreChanged(uint score)
    {
        _label.Text = "" + score;
    }
}