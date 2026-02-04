using Godot;
using System;
using System.Collections.Generic;

namespace CraterSprite
{
    public partial class InputManager : Node
    {
        public static InputManager instance { get; private set; }

        private SparseEventMap<string, float> _keyEventMap = new();

        public override void _Ready()
        {
            instance = this;
        }
        
        public void RegisterCallback(string actionName, Action<float> callback, Node owner)
        {
            _keyEventMap.RegisterCallback(actionName, callback);
            owner.TreeExited += () => RemoveCallback(actionName, callback);
        }

        public override void _Input(InputEvent @event)
        {
            if (!@event.IsActionType())
            {
                return;
            }

            if (@event.IsEcho())
            {
                return;
            }
            
            foreach (var actionName in _keyEventMap.GetMappedEvents())
            {
                if (!@event.IsAction(actionName))
                {
                    continue;
                }
                
                _keyEventMap.TriggerEvent(actionName, @event.GetActionStrength(actionName));
            }
        }

        private void RemoveCallback(string actionName, Action<float> callback)
        {
            _keyEventMap.RemoveCallback(actionName, callback);
            GD.Print("Unregistered callback");
        }
    }
}
