using Godot;
using System;
using System.Collections.Generic;

namespace CraterSprite
{
    public class SparseEventMap<TKey, TCallback>
    {
        private readonly Dictionary<TKey, List<Action<TCallback>>> _map = new ();

        public void RegisterCallback(TKey key, Action<TCallback> callback)
        {
            if (_map.TryGetValue(key, out var callbacks))
            {
                callbacks.Add(callback);
            }
            else
            {
                _map.Add(key, [callback]);
            }
        }

        public void TriggerEvent(TKey key, TCallback argument)
        {
            if (!_map.TryGetValue(key, out var callbacks))
            {
                return;
            }
            
            foreach (var callback in callbacks)
            {
                callback(argument);
            }
        }

        public bool RemoveCallback(TKey key, Action<TCallback> callback)
        {
            if (!_map.TryGetValue(key, out var callables))
            {
                return false;
                
            }

            if (!callables.Remove(callback))
            {
                return false;
            }
            
            if (callables.Count == 0)
            {
                _map.Remove(key);
            }

            return true;
        }

        public IEnumerable<TKey> GetMappedEvents()
        {
            return _map.Keys;
        }
    }

}
