using System;
using System.Collections.Generic;
using UnityEngine;

namespace Utils.EventManagement
{
    public class EventManager
    {
        #region PublicMethods

        public void AddEventListener(EventType eventType, Action listener)
        {
            _CheckAddingEvent(eventType, listener);
            _EventDictionary[eventType] = (Action)Delegate.Combine((Action)_EventDictionary[eventType], listener);
        }

        public void AddEventListener<T>(EventType eventType, Action<T> listener)
        {
            _CheckAddingEvent(eventType, listener);
            _EventDictionary[eventType] = (Action<T>)Delegate.Combine((Action<T>)_EventDictionary[eventType], listener);
        }

        public void RemoveEventListener(EventType eventType, Action listener)
        {
            if (_CheckRemovingEvent(eventType, listener))
                _EventDictionary[eventType] = (Action)Delegate.Remove((Action)_EventDictionary[eventType], listener);
        }


        public void RemoveEventListener<T>(EventType eventType, Action<T> listener)
        {
            if (_CheckRemovingEvent(eventType, listener))
                _EventDictionary[eventType] =
                    (Action<T>)Delegate.Remove((Action<T>)_EventDictionary[eventType], listener);
        }

        public void TriggerEvent(EventType eventType)
        {
            if (!_EventDictionary.TryGetValue(eventType, out var targetDelegate)) return;
            if (targetDelegate == null) return;

            var invocationList = targetDelegate.GetInvocationList();
            foreach (var listener in invocationList)
            {
                if (listener.GetType() != typeof(Action))
                {
                    Debug.LogError($"TriggerEvent {eventType} error : types of parameters are not match.");
                    throw new Exception();
                }

                var action = (Action)listener;

                try
                {
                    action?.Invoke();
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception.ToString());
                    throw;
                }
            }
        }

        public void TriggerEvent<T>(EventType eventType, T param)
        {
            if (!_EventDictionary.TryGetValue(eventType, out var targetDelegate)) return;
            if (targetDelegate == null) return;

            var invocationList = targetDelegate.GetInvocationList();
            foreach (var listener in invocationList)
            {
                if (listener.GetType() != typeof(Action<T>))
                {
                    Debug.LogError($"TriggerEvent {eventType} error : types of parameters are not match.");
                    throw new Exception();
                }

                var action = (Action<T>)listener;

                try
                {
                    action?.Invoke(param);
                }
                catch (Exception exception)
                {
                    Debug.LogError(exception.ToString());
                    throw;
                }
            }
        }

        #endregion PublicMethods

        #region PrivateMethods

        private void _CheckAddingEvent(EventType eventType, Delegate listener)
        {
            _EventDictionary.TryAdd(eventType, null);

            var tempDel = _EventDictionary[eventType];

            if (tempDel == null || tempDel.GetType() == listener.GetType()) return;

            Debug.LogError(
                $"try to add incorrect eventType {eventType}. needed listener type is {tempDel.GetType()}, given listener type is {listener.GetType()}");
            throw new Exception(
                $"try to add incorrect eventType {eventType}. needed listener type is {tempDel.GetType()}, given listener type is {listener.GetType()}");
        }

        private bool _CheckRemovingEvent(EventType eventType, Delegate listener)
        {
            var removeResult = false;
            if (!_EventDictionary.TryGetValue(eventType, out var tempDel))
            {
                removeResult = false;
            }
            else
            {
                if (tempDel != null && tempDel.GetType() != listener.GetType())
                {
                    Debug.LogError(
                        $"try to remove incorrect eventType {eventType}. needed listener type is {tempDel.GetType()}, given listener type is {listener.GetType()}");
                    throw new Exception(
                        $"try to remove incorrect eventType {eventType}. needed listener type is {tempDel.GetType()}, given listener type is {listener.GetType()}");
                }

                removeResult = true;
            }

            return removeResult;
        }

        #endregion PrivateMethods

        #region Fields

        private readonly Dictionary<EventType, Delegate> _EventDictionary = new();

        #endregion Fields
    }
}