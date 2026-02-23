using CraterSprite.Effects;
using CraterSprite.Match3;
using CraterSprite.Shared.Scripts;
using CraterSprite.Teams;
using Godot;
using ImGuiNET;

namespace CraterSprite;

/**
 * Class for holding relevant character stats information
 */
public partial class CharacterStats : Node, IDamageListener
{

    [Signal] public delegate void OnDeathEventHandler();
    [Signal] public delegate void OnTakeDamageEventHandler(float damage);
    [Signal] public delegate void OnStunnedEventHandler();
    [Signal] public delegate void OnStunEndEventHandler();

    [Export] private bool _showingStats;
    [Export] public Team characterTeam { private set; get; }
    [Export] public MatchType matchType { private set; get; }
    [Export] private float _defaultHealth = 15;
    [Export] private float _invulnerabilityTime = 0.0f;
    
    
    private readonly StatusEffectContainer _effects = new();
    
    public override void _Ready()
    {
        _effects.SetBaseValue(GameMode.instance.statusEffects.health, _defaultHealth);
        _effects.SetBaseValue(GameMode.instance.statusEffects.maxHealth, _defaultHealth);
        
        _effects.RegisterEffectAppliedCallback(GameMode.instance.statusEffects.invulnerability, EmitSignalOnStunned, this);
        _effects.RegisterEffectRemovedCallback(GameMode.instance.statusEffects.invulnerability, EmitSignalOnStunEnd, this);
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

    /**
     * <summary>
     *     Take damage. Automatically emits events associated with the health effect,
     *     and triggers the OnDeath signal if the damage causes this character to die
     * </summary>
     */
    public void TakeDamage(float damageAmount, CharacterStats source)
    {
        if (_effects.HasEffect(GameMode.instance.statusEffects.invulnerability))
        {
            GD.Print($"[CharacterStats] '{Owner.Name}' took damage, but was invulnerable so damage was discarded.");
            return;
        }
        
        GD.Print($"[CharacterStats] '{Owner.Name}' took '{damageAmount}' damage.");
        EmitSignalOnTakeDamage(damageAmount);
        var healthEffect = GameMode.instance.statusEffects.health;
        if (!(_effects.AddBaseValue(healthEffect, -damageAmount) <= 0.0f))
        {
            if (_invulnerabilityTime > 0.0f)
            {
                _effects.ApplyStatusEffectInstance(new StatusEffectInstance(GameMode.instance.statusEffects.invulnerability, this, _invulnerabilityTime));
            }
            return;
        }

        GD.Print($"Character '{Owner.Name}' died.");

        source?.KilledEnemy(this);
        Owner?.QueueFree();
        EmitSignalOnDeath();
    }

    public virtual void KilledEnemy(CharacterStats enemy)
    {
    }

    private void DrawImGui()
    {
        if (ImGui.Begin($"{Owner.GetName()} Status###HealthComponent{Owner.GetName()}"))
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
                    var content = "Source:";
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