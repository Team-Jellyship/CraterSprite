using System;

namespace CraterSprite.Teams;

[Flags]
public enum TeamFilter : byte
{
    Left = 1,
    Right = 2,
    Enemy = 4
}

public enum Team : byte
{
    Unaffiliated,
    Left,
    Right,
    Enemy
}

public static class TeamFunctions
{
    // This is kind of hacky, and only works because the teams are in the same
    // order as the team filter, and unaffiliated is 0
    public static bool TeamMatches(Team team, TeamFilter filter)
    {
        if (team == Team.Unaffiliated && filter == 0)
        {
            return true;
        }
        
        return (1 << (int)team - 1 & (int)filter) > 0;
    }
}