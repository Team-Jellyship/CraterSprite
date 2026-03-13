using Godot;

namespace CraterSprite;

[GlobalClass]
public partial class GameModeSettings : Resource
{
    [Export] public int roundsToWin = 3;
    [Export] public uint playerCount = 2;
    [Export] public PackedScene player;
    // Remove this later
    [Export] public PackedScene gem;
    [Export] public PackedScene defaultLevel;

    [ExportGroup("Scenes")]
    [Export] public PackedScene characterSelectScreen;
    [Export] public PackedScene victoryScreen;
    [Export] public PackedScene lossScreen;
    [Export] public PackedScene rematchScreen;
}