using CraterSprite.Game.GameMode;
using CraterSprite.Input;
using Godot;

namespace CraterSprite.UI.Menus;

public partial class StartMenu : Control
{
    public override void _Ready()
    {
        // Register this anonymous function with the Input Manager
        // We look for the action "ui_accept" from inside godot's input settings. In this case,
        // the anonymous function will only trigger if the input has been pressed -> "InputEventType.Pressed"
        InputManager.instance.RegisterCallback("accept", InputEventType.Pressed, _ =>
        { // Start of the anonymous function
            // Let the GameMode know to transition to the next GameState, as though it has registered a victory
            GameMode.instance.Command(GameModeCommand.Victory);
        }, // End of the anonymous function
            
            // We're listening on device 0 (the keyboard, or first controller)
            // and the callback's owner is this control node. The callback will unregister
            // the anonymous function we defined above when this object (StartMenu) is deleted
            0, this); 
    }
}