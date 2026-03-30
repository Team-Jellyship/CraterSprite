using Godot;
using System;
using CraterSprite.Game.GameMode;
using CraterSprite.Input;

namespace CraterSprite;

public partial class MainMenu : Control
{
    private bool _p1Started;
    private bool _p2Started;
    public override void _Ready()
    {
        InputManager.instance.RegisterCallback("start", InputEventType.Pressed, _ => P1Start(), 0, this);
        InputManager.instance.RegisterCallback("start", InputEventType.Pressed, _ => P2Start(), 1, this);
    }

    private void P1Start()
    {
        // if (_p1Started && _p2Started)
        {
            GameMode.instance.Command(GameModeCommand.Victory);
        }
        _p1Started = true;
    }
    
    private void P2Start()
    {
        if (_p1Started && _p2Started)
        {
            GameMode.instance.Command(GameModeCommand.Victory);
        }
        GameMode.instance.Command(GameModeCommand.Victory);
    }
}
