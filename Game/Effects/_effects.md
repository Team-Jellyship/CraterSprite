# Effects
### Relevant Classes
* [StatusEffect](StatusEffect.cs)
* [StatusEffectContainer](StatusEffectContainer.cs)
* [StatusEffectInstance](StatusEffectInstance.cs)
* [StatusEffectList](StatusEffectList.cs)
* [StatusEntry](StatusEntry.cs)


### What is an effect?
Effects represent different stats that can change for a character! Any stats that can change and either have a time or can be removed should be effects.  
For instance, if you want the player's max health to temporarily change when they are hit by a special attack, max health should be an effect.
Characters can set the base value of max health, and then the special attack can attach an instance of a Max Health [StatusEffect](StatusEffect.cs), called a [StatusEffectInstance](StatusEffectInstance.cs).

### Applying effects
To apply a [StatusEffect](StatusEffect.cs), create a new [StatusEffectInstance](StatusEffectInstance.cs)
and call `ApplyStatusEffectInstance` on the [StatusEffectContainer](StatusEffectContainer.cs). If you want to make a change to the base value without applying an effect, instead call `SetBaseValue`.

### Removing effects
To remove an effect, either store a reference to the [StatusEffectInstance](StatusEffectInstance.cs) and call `RemoveStatusEffectInstance` on the
relevant [StatusEffectContainer](StatusEffectContainer.cs), or set a time when applying a status effect instance -- status effect instances will
automatically expire, so long as the owner of the container calls `Update`.