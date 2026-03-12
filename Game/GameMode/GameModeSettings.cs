using Godot;

namespace CraterSprite;

[GlobalClass]
public partial class GameModeSettings : Resource
{
    [Export] public PackedScene player;
    [Export] public uint playerCount = 2;
    // Remove this later
    [Export] public PackedScene gem;
    [Export] public PackedScene defaultLevel;

    [Export] public PackedScene characterSelectScreen;
    [Export] public PackedScene victoryScreen;
    [Export] public PackedScene lossScreen;
}