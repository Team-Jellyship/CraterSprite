using Godot;
using System;
using System.Collections.Generic;

namespace CraterSprite
{
    public enum InputEventType
    {
        None,
        Pressed,
        Released,
        Changed
    }

    public struct InputAxis1D(string positive, string negative)
    {
        public readonly string negative = negative;
        public readonly string positive = positive;
    }
    
    public partial class InputManager : Node
    {
        private const bool TreatControllerAsSecondDevice = true;
        public static InputManager instance { get; private set; }

        private readonly SparseEventMap<Tuple<string, int>, float> _keyPressedEventMap = new();
        private readonly SparseEventMap<Tuple<string, int>, float> _keyReleasedEventMap = new();
        private readonly SparseEventMap<Tuple<string, int>, float> _keyChangedEventMap = new();
        private readonly SparseEventMap<Tuple<InputAxis1D, int>, float> _axisChangedEventMap = new();

        private readonly Dictionary<Key, string> _keyActionMap = new();
        private readonly Dictionary<JoyAxis, string> _gamepadAxisMap = new();
        private readonly Dictionary<JoyButton, string> _gamepadButtonMap = new();

        private readonly Dictionary<Tuple<string, int>, float> _actionDeviceValueMap = new();

        public override void _Ready()
        {
            instance = this;
            
            // Steal godot's existing action editor, so I don't have to write one
            foreach (var action in InputMap.GetActions())
            {
                if (action.ToString().StartsWith("ui_"))
                {
                    continue;
                }
                
                foreach (var inputEvent in InputMap.ActionGetEvents(action))
                {
                    switch (inputEvent)
                    {
                        case InputEventJoypadButton joypadEvent:
                            _gamepadButtonMap.Add(joypadEvent.ButtonIndex, action);
                            continue;
                        
                        case InputEventJoypadMotion joypadMotion:
                            _gamepadAxisMap[joypadMotion.Axis] = action;
                            continue;
                        
                        case InputEventKey key:
                            _keyActionMap.Add(key.PhysicalKeycode, action);
                            continue;
                    }
                }
            }
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
            GetMapForKeyType(type).RegisterCallback(new Tuple<string, int>(actionName, deviceId), callback);
            owner.TreeExited += () => RemoveCallback(actionName, type, deviceId, callback);
        }
        
        /**
         * <summary>Register an action-based axis callback</summary>
         * <param name="positive">Name of the positive input action, from the project input map</param>
         * <param name="negative">Name of the negative input action</param>
         * <param name="callback">Callback method to activate when the axis value changes</param>
         * <param name="deviceId">Device id to listen for. -1 listens for all devices</param>
         * <param name="owner">Node that is listening for this event. Required so InputManager can automatically unregister the callback</param>
         */
        public void RegisterAxisChangedCallback(string positive, string negative, Action<float> callback, int deviceId, Node owner)
        {
            GD.Print($"[InputManager] Registered 1D axis callback from action '{positive}' and '{negative}' on index {deviceId}");
            var axis = new InputAxis1D(positive, negative);
            var input = new Tuple<InputAxis1D, int>(axis, deviceId);
            _axisChangedEventMap.RegisterCallback(input, callback);
            owner.TreeExited += () => _axisChangedEventMap.RemoveCallback(input, callback);
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
            
            GD.Print($"action '{action}' pressed with strength '{strength}' for device {deviceId}");

            var key = new Tuple<string, int>(action, deviceId);
            if (_actionDeviceValueMap.TryGetValue(key, out var oldStrength) && Math.Abs(oldStrength - strength) < 0.0001f)
            {
                return;
            }
            
            _actionDeviceValueMap[key] = strength;
            
            foreach (var (axis, requestedDeviceId) in _axisChangedEventMap.GetMappedEvents())
            {
                if (axis.negative != action && axis.positive != action)
                {
                    continue;
                }

                _actionDeviceValueMap.TryGetValue(new Tuple<string, int>(axis.negative, deviceId), out var negative);
                _actionDeviceValueMap.TryGetValue(new Tuple<string, int>(axis.positive, deviceId), out var positive);
                _axisChangedEventMap.TriggerEvent(new Tuple<InputAxis1D, int>(axis, deviceId) , positive - negative);
                break;
            }
            
            if (inputEventType is InputEventType.Pressed or InputEventType.Released)
            {
                _keyChangedEventMap.TriggerEvent(key, _actionDeviceValueMap[key]);
            }
            
            GetMapForKeyType(inputEventType)?.TriggerEvent(key, strength);
        }

        private void RemoveCallback(string actionName, InputEventType type, int deviceId, Action<float> callback)
        {
            GetMapForKeyType(type).RemoveCallback(new Tuple<string, int>(actionName, deviceId), callback);
            GD.Print($"Unregistered '{type}' callback from action '{actionName}'");
        }

        private SparseEventMap<Tuple<string, int>, float> GetMapForKeyType(InputEventType type)
        {
            return type switch
            {
                InputEventType.Pressed => _keyPressedEventMap,
                InputEventType.Released => _keyReleasedEventMap,
                InputEventType.Changed => _keyChangedEventMap,
                _ => null
            };
        }

        private InputEventType GetEventType(InputEvent inputEvent)
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

        private bool GetActionFromEvent(InputEvent @event, out string action, out float strength)
        {
            switch (@event)
            {
                case InputEventJoypadButton joypadEvent:
                    strength = joypadEvent.Pressed ? 1.0f : 0.0f;
                    return _gamepadButtonMap.TryGetValue(joypadEvent.ButtonIndex, out action);
                        
                case InputEventJoypadMotion joypadMotion:
                    strength = joypadMotion.AxisValue;
                    if (strength < 0.2f)
                    {
                        strength = 0.0f;
                    }
                    return _gamepadAxisMap.TryGetValue(joypadMotion.Axis, out action);
                        
                case InputEventKey key:
                    strength = key.Pressed ? 1.0f : 0.0f;
                    return _keyActionMap.TryGetValue(key.Keycode, out action);
            }

            strength = 0.0f;
            action = "";
            return false;
        }
    }
}
