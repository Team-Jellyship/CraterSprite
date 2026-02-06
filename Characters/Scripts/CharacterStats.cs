using Godot;
using System;
using CraterSprite.Effects;
using ImGuiNET;

namespace CraterSprite;

public enum Team
{
    Left,
    Right,
    Enemy,
    Unaffiliated
}


/**
 * Class for holding relevant character stats information
 */
public partial class CharacterStats : Node
{
    private readonly StatusEffectContainer _effects = new();

    [Export] private bool _showingStats = false;

    public override void _Ready()
    {
        _effects.SetBaseValue(GameMode.instance.statusEffects.health, 15);
        _effects.SetBaseValue(GameMode.instance.statusEffects.maxHealth, 15);
    }

    public override void _Process(double delta)
    {
        _effects.Update((float)delta);

        if (_showingStats)
        {
            DrawImGui();
        }
    }

    public void ToggleShowStats()
    {
        _showingStats = !_showingStats;
    }

    public void TakeDamage(float damageAmount)
    {
        var healthEffect = GameMode.instance.statusEffects.health;
        _effects.AddBaseValue(healthEffect, -damageAmount);
    }

    private void DrawImGui()
    {
        if (ImGui.Begin($"{GetName()} Status###HealthComponent{GetName()}"))
        {
            ImGui.Text($"Health: {_effects.GetValue(GameMode.instance.statusEffects.health)} / {_effects.GetValue(GameMode.instance.statusEffects.maxHealth)}");
            foreach (var item in _effects)
            {
                var title = item.Key.accumulator switch
                {
                    EffectAccumulator.None =>
                        $"{item.Key.effectName}: {item.Value.count} stacks###HealthComponent{item.Key.effectName}",
                    EffectAccumulator.BaseValueOnly =>
                        $"{item.Key.effectName}: {item.Value.Accumulate()} ###HealthComponent{item.Key.effectName}",
                    _ =>
                        $"{item.Key.effectName}: {item.Value.Accumulate()} - {item.Value.count} stack(s)###HealthComponent{item.Key.effectName}"
                };
                if (!ImGui.TreeNode(title))
                {
                    continue;
                }

                if (item.Key.accumulator != EffectAccumulator.None)
                {
                    ImGui.Text($"Base value: {item.Value.baseValue}");
                }

                foreach (var instance in item.Value)
                {
                    var content = $"Source:";
                    if (instance.duration != 0.0f)
                    {
                        content += $"\tTime: {instance.currentTime}/{instance.duration}s";
                    }

                    if (instance.strength != 0.0f)
                    {
                        content += $"\tStrength: {instance.strength}";
                    }

                    ImGui.Text(content);
                }
                ImGui.TreePop();
            }
        }

        ImGui.End();
    }
}
