using System.Collections.Generic;
using Godot;

namespace CraterSprite;

public static class CraterFunctions
{
    public static T GetNodeByClass<T>(Node parent)
        where T : Node
    {
        foreach (var child in parent.GetChildren())
        {
            if (child is T node)
            {
                return node;
            }
        }

        return null;
    }

    public static T GetNodeByClassFromParent<T>(Node self)
        where T : Node
    {
        var parent = self.GetParent();
        return parent == null ? null : GetNodeByClass<T>(parent);
    }

    public static T GetNodeByClassFromRoot<T>(Node self)
        where T : Node
    {
        var root = self.Owner;
        return root == null ? null : GetNodeByClass<T>(root);
    }

    public static T CreateInstance<T>(Node rootContext, PackedScene prefab, Vector2 position)
        where T : Node2D
    {
        if (prefab == null)
        {
            return null;
        }

        var newInstance = prefab.Instantiate<T>();
        rootContext.GetTree().GetRoot().AddChild(newInstance);
        newInstance.SetGlobalPosition(position);
        return newInstance;
    }
    
    public static T CreateInstanceDeferred<T>(Node rootContext, PackedScene prefab, Vector2 position)
        where T : Node2D
    {
        if (prefab == null)
        {
            return null;
        }

        var newInstance = prefab.Instantiate<T>();
        rootContext.GetTree().GetRoot().CallDeferred("add_child", newInstance);
        newInstance.SetGlobalPosition(position);
        return newInstance;
    }

    public static List<T> ConvertArray<[MustBeVariant] T>(Godot.Collections.Array<T> array)
    {
        var rawArray = new T[array.Count];
        array.CopyTo(rawArray, 0);
        return [..rawArray];
    }

    public static Godot.Collections.Array<T> ConvertToGodotArray<[MustBeVariant] T>(List<T> list)
    {
        Godot.Collections.Array<T> godotArray = [];
        godotArray.AddRange(list);
        return godotArray;
    }
}