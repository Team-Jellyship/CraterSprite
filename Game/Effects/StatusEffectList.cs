using Godot;

namespace CraterSprite.Effects;

[GlobalClass]
public partial class StatusEffectList : Resource
{
    [Export] public StatusEffect health { get; private set; }
    [Export] public StatusEffect maxHealth { get; private set; }
    [Export] public StatusEffect invulnerability { get; private set; }
}