using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite.UI.HUD.Scripts;

public partial class PlayerHealthDisplay : AutobindUiElement
{
    [Export] private Label _healthText;
    protected override void Bind(PlayerState playerState)
    {
        playerState.RegisterEffectChangedDelegate(GameMode.instance.statusEffects.health, HealthChanged, this);
        var currentHealth = playerState.GetEffectValue(GameMode.instance.statusEffects.health);
        _healthText.Text =$"hp: {currentHealth}";
    }

    private void HealthChanged(int stacks, float newHealth)
    {
        _healthText.Text = $"hp: {newHealth}";
    }
}