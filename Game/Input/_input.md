# Input System

### Input flow
Godot by default sends input events to every node, and the nodes are responsible for looking
up the event with the [IsAction](https://docs.godotengine.org/en/stable/classes/class_inputevent.html#class-inputevent-method-is-action) function.
This is a really **bad** approach for a number of reasons, so I've tried to fix some of them here.

1. The [InputManager](InputManager.cs) class handles all key/gamepad events
2. If an input event is received, and indicates a change in input (i.e., not ghost inputs), looks for a relevant action string
3. If the action is an axis type, the new axis value will automatically be calculated
4. If an action string is found, the event calls all `Actions` bound to the action name and device id
   * Change functions will always emit if the underlying input value has changed
   * Pressed functions will emit if the value has been pressed (if relevant -- input axes don't have press/release!)
   * Release functions will emit if the value has been released