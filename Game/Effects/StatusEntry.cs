using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CraterSprite.Effects;

public class StatusEntry : IEnumerable<StatusEffectInstance>
{
    public List<StatusEffectInstance> instances;
    public float baseValue;
    public StatusEffect statusEffect;

    public StatusEntry(StatusEffect effect, float baseValue)
    {
        this.statusEffect = effect;
        this.baseValue = baseValue;
        instances = [];
    }

    public bool HasExpired()
    {
        return baseValue == 0.0f && instances.Count == 0;
    }

    public void Add(StatusEffectInstance instance)
    {
        instances.Add(instance);
    }

    public bool Remove(StatusEffectInstance instance)
    {
        return instances.Remove(instance);
    }

    public IEnumerator<StatusEffectInstance> GetEnumerator()
    {
        return instances.GetEnumerator();
    }

    public int RemoveExpired(float deltaSeconds)
    {
        return instances.RemoveAll(effectInstance => effectInstance.UpdateCheckExpiration(deltaSeconds));
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return instances.GetEnumerator();
    }

    public int count => instances.Count;
    
    public float Accumulate()
    {
        if (instances.Count == 0)
        {
            return baseValue;
        }
        
        return statusEffect.accumulator switch
        {
            EffectAccumulator.None => 0.0f,
            EffectAccumulator.BaseValueOnly => baseValue,
            EffectAccumulator.Additive => baseValue + instances.Sum(instance => instance.strength),
            EffectAccumulator.Maximum => Math.Max(baseValue, instances.Max(instance => instance.strength)),
            EffectAccumulator.Minimum => Math.Min(baseValue, instances.Min(instance => instance.strength)),
            EffectAccumulator.Multiplicative => baseValue * instances.Aggregate(1.0f, (current, instance) => current * instance.strength),
            _ => 0.0f,
        };
    }
}