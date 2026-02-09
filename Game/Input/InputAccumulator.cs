using Godot;

namespace CraterSprite.Input;

using InputVariant = Variant<Key, JoyAxis, JoyButton>;

public enum InputMappingType : byte
{
    None,
    Positive,
    Negative,
    Range
}

public class InputAccumulator(InputVariant input, InputMappingType mappingType)
{
    public InputVariant input = input;
    public InputMappingType mappingType = mappingType;

    public float Map(float baseInputValue)
    {
        return mappingType switch
        {
            InputMappingType.None => 0.0f,
            InputMappingType.Negative => -baseInputValue,
            InputMappingType.Positive => baseInputValue,
            InputMappingType.Range => baseInputValue,
            _ => 0.0f
        };
    }
}