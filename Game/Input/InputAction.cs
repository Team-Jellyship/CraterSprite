using System.Collections.Generic;
using Godot;

namespace CraterSprite.Input;

public class InputAction(string name)
{
    public readonly string name = name;

    public readonly List<InputAccumulator> accumulators = [];
}