using Godot;

namespace CraterSprite;

[GlobalClass]
public partial class GameModeSettings : Resource
{
    [Export] public PackedScene player;
    [Export] public uint playerCount = 2;
    [Export] public PackedScene gem;
}