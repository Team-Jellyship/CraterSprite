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
}