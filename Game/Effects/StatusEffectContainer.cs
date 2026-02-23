using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace CraterSprite.Effects;

public class StatusEffectContainer
{
    private Dictionary<StatusEffect, StatusEntry> _statusEffects = new ();

    private SparseEventMap<StatusEffect> _onStatusEffectAppliedMap = new();
    private SparseEventMap<StatusEffect> _onStatusEffectRemovedMap = new();
    private SparseEventMap<StatusEffect, int, float> _onStatusEffectStacksChangedMap = new();

    public void ApplyStatusEffectInstance(StatusEffectInstance instance)
    {
        if (instance.effect == null)
        {
            return;
        }
        
        if (_statusEffects.TryGetValue(instance.effect, out var effectList))
        {
            effectList.Add(instance);
        }
        else
        {
            // Create the stack in our dictionary, and invoke the event for a new stack here
            // if the event exists. Assume no one has subscribed if the event does not exist
            effectList = new StatusEntry(instance.effect, 0.0f) { instance };
            _statusEffects.Add(instance.effect, effectList);
            _onStatusEffectAppliedMap.TriggerEvent(instance.effect);
        }
        
        _onStatusEffectStacksChangedMap.TriggerEvent(instance.effect, effectList.count, effectList.Accumulate());
    }

    public void SetBaseValue(StatusEffect effect, float newBaseValue)
    {
        if (!_statusEffects.TryGetValue(effect, out var effectList))
        {
            _statusEffects.Add(effect, new StatusEntry(effect, newBaseValue));
            return;
        }

        // If the old base value is the same, don't signal a change event
        // Comparison check with floating point error
        if (Math.Abs(effectList.baseValue - newBaseValue) < 0.001f)
        {
            return;
        }

        effectList.baseValue = newBaseValue;
        _onStatusEffectStacksChangedMap.TriggerEvent(effect, effectList.count, effectList.Accumulate());
    }

    public void RemoveStatusEffectInstance(StatusEffectInstance instance)
    {
        if (!_statusEffects.TryGetValue(instance.effect, out var effectList))
        {
            return;
        }

        if (!effectList.Remove(instance))
        {
            return;
        }
        
        _onStatusEffectStacksChangedMap.TriggerEvent(instance.effect, effectList.count, effectList.Accumulate());

        if (!effectList.HasExpired())
        {
            return;
        }
        
        _statusEffects.Remove(instance.effect);
        _onStatusEffectRemovedMap.TriggerEvent(instance.effect);
    }

    public void Update(float deltaSeconds)
    {
        var effectsToRemove = new List<StatusEffect>();
        foreach (var stackList in _statusEffects)
        {
            var numRemoved = stackList.Value.RemoveExpired(deltaSeconds);
            if (numRemoved > 0)
            {
                _onStatusEffectStacksChangedMap.TriggerEvent(stackList.Key, stackList.Value.count, stackList.Value.Accumulate());
            }
            
            if (stackList.Value.HasExpired())
            {
                effectsToRemove.Add(stackList.Key);
            }
        }

        foreach (var effectToRemove in effectsToRemove)
        {
            _onStatusEffectRemovedMap.TriggerEvent(effectToRemove);
            _statusEffects.Remove(effectToRemove);
        }
    }

    public override string ToString()
    {
        var result = new string("");

        return _statusEffects.Aggregate(result, (current, statusEffect)
            => current + $"{statusEffect.Key.ResourceName}: '{statusEffect.Value.count}'");
    }

    public float GetValue(StatusEffect effect)
    {
        return !_statusEffects.TryGetValue(effect, out var effectList) ? 0.0f : effectList.Accumulate();
    }

    public float AddBaseValue(StatusEffect effect, float delta)
    {
        if (_statusEffects.TryGetValue(effect, out var effectList))
        {
            effectList.baseValue += delta;
            var newValue = effectList.Accumulate();
            _onStatusEffectStacksChangedMap.TriggerEvent(effect, effectList.count, newValue);
            return newValue;
        }
        
        SetBaseValue(effect, delta);
        return delta;
    }

    public Dictionary<StatusEffect, StatusEntry>.Enumerator GetEnumerator()
    {
        return _statusEffects.GetEnumerator();
    }

    public void RegisterEffectAppliedCallback(StatusEffect effect, Action action, Node owner)
    {
        _onStatusEffectAppliedMap.RegisterCallback(effect, action);
        owner.TreeExited += () => _onStatusEffectAppliedMap.RemoveCallback(effect, action);
    }

    public void RegisterEffectRemovedCallback(StatusEffect effect, Action action, Node owner)
    {
        _onStatusEffectRemovedMap.RegisterCallback(effect, action);
        owner.TreeExited += () => _onStatusEffectRemovedMap.RemoveCallback(effect, action);
    }

    public void RegisterStatusEffectChangedEvent(StatusEffect effect, Action<int, float> action, Node owner)
    {
        _onStatusEffectStacksChangedMap.RegisterCallback(effect, action);
        owner.TreeExited += () => _onStatusEffectStacksChangedMap.RemoveCallback(effect, action);
    }

    public bool HasEffect(StatusEffect effect)
    {
        return effect != null && _statusEffects.ContainsKey(effect);
    }
}