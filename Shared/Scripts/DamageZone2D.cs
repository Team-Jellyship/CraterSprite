using CraterSprite.Teams;
using Godot;

namespace CraterSprite;

public partial class DamageZone2D : Area2D
{
    [Export] private TeamFilter _filter;
    [Export] private float _damage;
    [Export] private CharacterStats _owner;
    
    public override void _Ready()
    {
        AreaEntered += Overlap;
    }

    private void Overlap(Area2D area)
    {
        var characterStats = CraterFunctions.GetNodeByClassFromRoot<CharacterStats>(area);
        if (characterStats == null)
        {
            return;
        }

        if (!TeamFunctions.TeamMatches(characterStats.characterTeam, _filter))
        {
            return;
        }

        if (_owner != null && _owner == characterStats)
        {
            return;
        }
        
        characterStats.TakeDamage(_damage, null);
    }
}