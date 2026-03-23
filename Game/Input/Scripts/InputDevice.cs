using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using ImGuiNET;

namespace CraterSprite.Input;

using InputVariant = Variant<Key, JoyAxis, JoyButton>;

public class InputDevice
{
    private readonly Dictionary<InputVariant, float> _deviceValueMap = new();
    
    private readonly SparseEventMap<InputAction, float> _keyPressedEventMap = new();
    private readonly SparseEventMap<InputAction, float> _keyReleasedEventMap = new();
    private readonly SparseEventMap<InputAction, float> _keyChangedEventMap = new();
    
    public void RegisterCallback(InputAction inputAction, InputEventType type, Action<float> callback, Node owner)
    {
        var map = GetMapForKeyType(type);
        map.RegisterCallback(inputAction, callback);
        owner.TreeExited += () => map.RemoveCallback(inputAction, callback);
    }
    
    public void HandleInput(InputVariant input, InputEventType inputEventType, InputAction action, float strength)
    {
        _deviceValueMap[input] = strength;
        var newStrength = Accumulate(action);
                
        if (inputEventType is InputEventType.Pressed or InputEventType.Released)
        {
            _keyChangedEventMap.TriggerEvent(action, newStrength);
        }
                    
        GetMapForKeyType(inputEventType)?.TriggerEvent(action, newStrength);
    }

    private float Accumulate(InputAction action)
    {
        return action.accumulators.Sum(accumulator => accumulator.Map(_deviceValueMap.GetValueOrDefault(accumulator.input, 0.0f)));
    }
    
    private SparseEventMap<InputAction, float> GetMapForKeyType(InputEventType type)
    {
        return type switch
        {
            InputEventType.Pressed => _keyPressedEventMap,
            InputEventType.Released => _keyReleasedEventMap,
            InputEventType.Changed => _keyChangedEventMap,
            _ => null
        };
    }

    public void DrawActions(int deviceId)
    {
        if (ImGui.Begin($"Input Device: {deviceId}"))
        {
            foreach (var action in InputManager.instance.actions)
            {
                var accumulatedValue = Accumulate(action);
                if (ImGui.TreeNode($"{action.name}: {accumulatedValue}###{action.name}"))
                {
                    foreach (var accumulator in action.accumulators)
                    {
                        ImGui.Text($"{accumulator.input}: {accumulator.Map(_deviceValueMap.GetValueOrDefault(accumulator.input, 0.0f))}");
                    }
                    ImGui.TreePop();
                }
            }
        }
        ImGui.End();

    }
}