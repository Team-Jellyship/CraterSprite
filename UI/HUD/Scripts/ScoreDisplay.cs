using Godot;

namespace CraterSprite.UI.HUD.Scripts;

public partial class ScoreDisplay : AutobindUiElement
{
    [Export] private ProgressBar _progressBar;

    protected override void Bind(PlayerState playerState)
    {
        playerState.onSuperchargeChanged.AddListener((charge, maxCharge) =>
        {
            _progressBar.Value = charge;
            _progressBar.MaxValue = maxCharge;
        });
    }
}