using Godot;
using System;
using System.Collections.Generic;
using ImGuiNET;

namespace CraterSprite.Input
{
    using InputVariant = Variant<Key, JoyAxis, JoyButton>;

    public enum InputEventType
    {
        None,
        Pressed,
        Released,
        Changed
    }

    public struct InputAxis1D(InputAction positive, InputAction negative)
    {
        public readonly InputAction negative = negative;
        public readonly InputAction positive = positive;
    }
    
    public partial class InputManager : Node
    {
        private const bool TreatControllerAsSecondDevice = true;
        public static InputManager instance { get; private set; }

        // Map our keys to action strings
        private readonly Dictionary<InputVariant, InputAction> _keyActionMap = new();
        public readonly List<InputAction> actions = [];
        private readonly List<InputDevice> _devices = [new(), new()];

        private bool _showDebug;

        public override void _Ready()
        {
            instance = this;
            var actionCount = 0;
            // Steal godot's existing action editor, so I don't have to write one
            foreach (var action in InputMap.GetActions())
            {
                if (action.ToString().StartsWith("ui_"))
                {
                    continue;
                }

                ++actionCount;
                foreach (var inputEvent in InputMap.ActionGetEvents(action))
                {
                    var inputVariant = GetInputVariantFromEvent(inputEvent);
                    _keyActionMap.Add(inputVariant, AddAccumulator(inputVariant, action));
                }
            }
            
            GD.Print($"[Input Manager] Parsed {actions.Count} unique actions from {actionCount} in the action map.");
        }

        public override void _Input(InputEvent @event)
        {
            var inputEventType = GetEventType(@event);
            if (inputEventType == InputEventType.None)
            {
                return;
            }

            var deviceId = @event.Device;
            if (TreatControllerAsSecondDevice)
            {
                if (@event is InputEventJoypadButton or InputEventJoypadMotion)
                {
                    ++deviceId;
                }
            }

            if (!GetActionFromEvent(@event, out var action, out var strength))
            {
                return;
            }
            
            // GD.Print($"action '{action}' pressed with strength '{strength}' for device {deviceId}");
            _devices[deviceId].HandleInput(GetInputVariantFromEvent(@event), GetEventType(@event), action, strength);
        }
        
        /**
         * <summary>Register an action-based callback</summary>
         * <param name="actionName">Name of the action, from the project input map</param>
         * <param name="type">Type of event to listen to</param>
         * <param name="callback">Callback method to activate when event is triggered. Strength will be 1 when pressed, and 0 when released</param>
         * <param name="deviceId">Device id to listen for. -1 listens for all devices</param>
         * <param name="owner">Node that is listening for this event. Required so InputManager can automatically unregister the callback</param>
         */
        public void RegisterCallback(string actionName, InputEventType type, Action<float> callback, int deviceId, Node owner)
        {
            if (!TryGetAction(actionName, out var action))
            {
                return;
            }
            _devices[deviceId].RegisterCallback(action, type, callback, owner);
        }
        
        private static InputEventType GetEventType(InputEvent inputEvent)
        {
            if (inputEvent.IsEcho() || !inputEvent.IsActionType())
            {
                return InputEventType.None;
            }

            if (inputEvent.IsPressed())
            {
                return InputEventType.Pressed;
            }

            if (inputEvent.IsReleased())
            {
                return InputEventType.Released;
            }

            return InputEventType.Changed;
        }

        /**
         * <summary>Get the InputAction associated with a Godot InputEvent, and its strength</summary>
         * <param name="event">Godot InputEvent</param>
         * <param name="action">Action associated with event. Null if no action is associated with this event</param>
         * <param name="strength">Input strength. 0.0f if there is no associated strength</param>
         */
        private bool GetActionFromEvent(InputEvent @event, out InputAction action, out float strength)
        {
            switch (@event)
            {
                case InputEventJoypadButton joypadEvent:
                    strength = joypadEvent.Pressed ? 1.0f : 0.0f;
                    return _keyActionMap.TryGetValue(joypadEvent.ButtonIndex, out action);
                        
                case InputEventJoypadMotion joypadMotion:
                    strength = joypadMotion.AxisValue;
                    if (Mathf.Abs(strength) < 0.2f)
                    {
                        strength = 0.0f;
                    }
                    return _keyActionMap.TryGetValue(joypadMotion.Axis, out action);
                        
                case InputEventKey key:
                    strength = key.Pressed ? 1.0f : 0.0f;
                    return _keyActionMap.TryGetValue(key.Keycode, out action);
            }

            strength = 0.0f;
            action = null;
            return false;
        }

        private static InputVariant GetInputVariantFromEvent(InputEvent @event)
        {
            return @event switch
            {
                InputEventKey key => new InputVariant(key.PhysicalKeycode),
                InputEventJoypadButton button => new InputVariant(button.ButtonIndex),
                InputEventJoypadMotion motion => new InputVariant(motion.Axis),
                _ => null
            };
        }

        private bool TryGetAction(string actionName, out InputAction action)
        {
            action = actions.Find((inputAction => inputAction.name == actionName));
            return action != null;
        }

        private InputAction CreateOrGetAction(string actionName)
        {
            var existingAction = actions.Find((inputAction => inputAction.name == actionName));
            if (existingAction != null)
            {
                return existingAction;
            }

            var action = new InputAction(actionName);
            actions.Add(action);
            return action;
        }

        private InputAction AddAccumulator(InputVariant key, string actionName)
        {
            var processedName = actionName;
            var mappingType = InputMappingType.Positive;
            if (actionName.EndsWith('+'))
            {
                processedName = processedName[..^1];
            }
            else if (actionName.EndsWith('-'))
            {
                processedName = processedName[..^1];
                mappingType = InputMappingType.Negative;
            }
            else if (actionName.EndsWith('_'))
            {
                processedName = processedName[..^1];
                mappingType = InputMappingType.Range;
            }
            
            var action = CreateOrGetAction(processedName);
            action.accumulators.Add(new InputAccumulator(key, mappingType));
            return action;
        }

        public void ToggleDebug()
        {
            _showDebug = !_showDebug;
        }

        public override void _Process(double delta)
        {
            if (!_showDebug)
            {
                return;
            }
            
            // Preprocessor guards aren't working right now
// #if IMGUI
            for (var i = 0; i < _devices.Count; ++i)
            {
                _devices[i].DrawActions(i);
            }

            DrawInputActions();
// #endif
        }

// #if IMGUI
        public void DrawInputActions()
        {
            if (ImGui.Begin("InputActions"))
            {
                foreach (var action in actions)
                {
                    if (ImGui.TreeNodeEx(action.name, ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        foreach (var accumulator in action.accumulators)
                        {
                            ImGui.Text($"{GetInputVariantAsString(accumulator.input)} : {accumulator.mappingType}");
                        }
                        ImGui.TreePop();
                    }
                }
            }
            ImGui.End();
        }
// #endif

        /**
         * <summary>
         * Convert an InputVariant into a human-readable string
         * </summary>
         */
        public static string GetInputVariantAsString(InputVariant inputVariant)
        {
            return inputVariant.index switch
            {
                0 => $"Key {inputVariant}",
                1 or 2 => $"Gamepad {inputVariant}",
                _ => "Err"
            };
        }
    }
}
