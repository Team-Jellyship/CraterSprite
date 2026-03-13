using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.UI.Menus;

public partial class RematchMenu : Control
{
    [Export] private Button _rematchButton;

    public override void _Ready()
    {
        _rematchButton.GrabFocus();
        _rematchButton.ButtonDown += RematchPressed;
    }

    private void RematchPressed()
    {
        _rematchButton.ButtonDown -= RematchPressed;
        GameMode.instance.Command(GameModeCommand.Victory);
    }
}