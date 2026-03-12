using Godot;

namespace CraterSprite.Input;

[GlobalClass]
public partial class InputSettings : Resource
{
    // Should the first connected gamepad be considered the second input device?
    // If true, gamepad 1 will be control the second player
    [Export] public bool treatGamepadAsSecondDevice = true;
}