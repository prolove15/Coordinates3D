using System.Collections.Generic;
using UnityEngine;

namespace EazyCamera.Events
{
    using Util = EazyCameraUtility;

    public delegate void EventCallback(EventData data);

    public interface IEventSource
    {
        void BroadcastEvent(string key, EventData data);
    }

    public interface IEventListener
    {
        void BindEvents();
        void UnbindEvents();
    }

    public class EazyEvent
    {
        public string Name => _name;
        private readonly string _name = string.Empty;

        private event EventCallback _onTriggered = null;

        public EazyEvent(string name)
        {
            _name = name;
            _onTriggered = Util.NoOp;
        }

        public EazyEvent(string name, EventCallback defaultCallback)
        {
            _name = name;
            _onTriggered = defaultCallback;
        }

        public void TriggerEvent(EventData data)
        {
            _onTriggered?.Invoke(data);
        }

        public void BindCallback(EventCallback callback)
        {
            _onTriggered += callback;
        }

        public void UnbindCallback(EventCallback callback)
        {
            _onTriggered -= callback;
        }
    }


    public static class EazyEventManager
    {
        private static Dictionary<string, EazyEvent> _eventMappings = new Dictionary<string, EazyEvent>();

        public static void BindToEvent(string eventKey, EventCallback callback)
        {
            if (_eventMappings.TryGetValue(eventKey, out EazyEvent evt))
            {
                evt.BindCallback(callback);
            }
            else
            {
                evt = new EazyEvent(eventKey);
                evt.BindCallback(callback);
                _eventMappings.Add(eventKey, evt);
            }
        }

        public static void UnbindFromEvent(string eventKey, EventCallback callback)
        {
            if (_eventMappings.TryGetValue(eventKey, out EazyEvent evt))
            {
                evt.UnbindCallback(callback);
            }
            else
            {
                Debug.LogWarning("[EazyEventManager::UnbindFromEvent] Attempting to unind from an event that has not been registered");
            }
        }

        public static void TriggerEvent(string eventKey, EventData data)
        {
            if (_eventMappings.TryGetValue(eventKey, out EazyEvent evt))
            {
                evt.TriggerEvent(data);
            }
            else
            {
                Debug.LogWarning("[EazyEventManager::TriggerEvent] Attempting to trigger an event that has not been registered");
            }
        }
    }
}
