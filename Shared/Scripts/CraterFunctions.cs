using System;
using System.Collections.Generic;
using System.Linq;
using CraterSprite.Game.GameMode;
using Godot;

namespace CraterSprite;

public static class CraterFunctions
{
    /**
     * <summary> Get the first node of Class T that is an immediate child</summary>
     * <typeparam name="T">Node class to get. Can be any type, in order to accept
     * interfaces</typeparam>
     * <param name="parent">Node to search from</param>
     * <returns>Found node, or null if no node exists. If T is not a node type,
     * returns the default value, which may not be null!</returns>
     */
    public static T GetNodeByClass<T>(Node parent)
    {
        foreach (var child in parent.GetChildren())
        {
            if (child is T node)
            {
                return node;
            }
        }
        return default;
    }

    /**
     * <summary>Get a list of all nodes of class T that are immediate children</summary>
     * <typeparam name="T">Node class to get</typeparam>
     * <param name="parent">Node to search from</param>
     * <returns>Typed List, containing all nodes. Will be empty if no nodes exist</returns>
     */
    public static List<T> GetAllNodesByClass<T>(Node parent)
    {
        return parent == null ? [] : parent.GetChildren().OfType<T>().ToList();
    }

    /**
     * <summary>Helper function to call <see cref="GetNodeByClass"/> on the parent of this node</summary>
     */
    public static T GetNodeByClassFromParent<T>(Node self)
    {
        var parent = self.GetParent();
        return parent == null ? default : GetNodeByClass<T>(parent);
    }

    /**
     * <summary>Helper function to call <see cref="GetNodeByClass"/> on the root of this node</summary>
     */
    public static T GetNodeByClassFromRoot<T>(Node self)
    {
        var root = self.Owner;
        return root == null ? default : GetNodeByClass<T>(root);
    }

    /**
     * <summary> Get the first node of Class T that is a descendant of this node.
     * Executes in breadth-first order. If the node is guaranteed to be a child of this,
     * prefer <see cref="GetNodeByClass"/> instead.</summary>
     * <typeparam name="T">Node class to get. Can be any type, in order to accept
     * interfaces</typeparam>
     * <param name="root">Node to search from</param>
     * <returns>Found node, or null if no node exists. If T is not a node type,
     * returns the default value, which may not be null!</returns>
     */
    public static T FindNodeByClass<T>(Node root)
    {
        var nodesToSearch = root.GetChildren();
        for (var i = 0; i < nodesToSearch.Count; ++i)
        {
            var node = nodesToSearch[i];
            if (node is T typedNode)
            {
                return typedNode;
            }
            nodesToSearch.AddRange(node.GetChildren());
        }
        return default;
    }

    /**
     * <summary>Create an instance of a Node, and add it to the current level, defined in the current GameMode instance</summary>
     * <typeparam name="T">Expected type of the prefab's root node</typeparam>
     * <param name="prefab">Prefab to instantiate</param>
     * <param name="position">Position to place the prefab, in the current level</param>
     * <returns>Root node of the prefab, or null if the prefab failed to construct for some reason</returns>
     */
    public static T CreateInstance<T>(PackedScene prefab, Vector2 position)
        where T : Node2D
    {
        if (prefab == null)
        {
            return null;
        }

        var newInstance = prefab.Instantiate<T>();
        // rootContext.GetTree().GetRoot().AddChild(newInstance);
        GameMode.instance.worldRoot.AddChild(newInstance);
        newInstance.SetGlobalPosition(position);
        return newInstance;
    }
    
    public static T CreateInstanceDeferred<T>(PackedScene prefab, Vector2 position)
        where T : Node2D
    {
        if (prefab == null)
        {
            return null;
        }

        var newInstance = prefab.Instantiate<T>();
        Callable.From(() =>
        {
            GameMode.instance.worldRoot.AddChild(newInstance);
        }).CallDeferred();
        
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

    public static Timer CreateTimer(Node parent, string timerName, Action timerExpiredCallback)
    {
        var timer = new Timer();
        parent.AddChild(timer);
        timer.SetName(timerName);
        timer.OneShot = true;
        timer.Timeout += timerExpiredCallback;
        return timer;
    }

    public static Timer CreateAndStartTimer(Node parent, string timerName, float time, Action timerExpiredCallback)
    {
        var timer = CreateTimer(parent, timerName, timerExpiredCallback);
        timer.Start(time);
        return timer;
    }
    
}