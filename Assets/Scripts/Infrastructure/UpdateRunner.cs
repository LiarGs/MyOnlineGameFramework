using System;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public class UpdateRunner : MonoBehaviour
    {
        #region UnityBehavior
        /// <summary>
        /// Each frame, advance all subscribers. Any that have hit their period should then act, though if they take too long they could be removed.
        /// </summary>
        private void Update()
        {
            while (_PendingHandlers.Count > 0)
            {
                _PendingHandlers.Dequeue()?.Invoke();
            }

            foreach (var (subscriber, subscriberData) in _SubscriberData)
            {
                if (Time.time < subscriberData.NextCallTime) continue;
                
                subscriber.Invoke(Time.time - subscriberData.LastCallTime);
                subscriberData.LastCallTime = Time.time;
                subscriberData.NextCallTime = Time.time + subscriberData.Period;
            }
        }
        
        public void OnDestroy()
        {
            _PendingHandlers.Clear();
            _SubscriberData.Clear();
        }

        #endregion UnityBehavior

        /// <summary>
        /// Subscribe in order to have onUpdate called approximately every period seconds (or every frame, if period <= 0).
        /// Don't assume that onUpdate will be called in any particular order compared to other subscribers.
        /// </summary>
        public void Subscribe(Action<float> onUpdate, float updatePeriod)
        {
            if (onUpdate == null)
            {
                return;
            }

            if (onUpdate.Target == null) // Detect a local function that cannot be Unsubscribed since it could go out of scope.
            {
                Debug.LogError("Can't subscribe to a local function that can go out of scope and can't be unsubscribed from");
                return;
            }

            if (onUpdate.Method.ToString().Contains("<")) // Detect
            {
                Debug.LogError("Can't subscribe with an anonymous function that cannot be Unsubscribed, by checking for a character that can't exist in a declared method name.");
                return;
            }

            if (!_SubscriberData.ContainsKey(onUpdate))
            {
                _PendingHandlers.Enqueue(() =>
                {
                    _SubscriberData.Add(onUpdate, new SubscriberData() { Period = updatePeriod, NextCallTime = 0, LastCallTime = Time.time });
                });
            }
        }

        /// <summary>
        /// Safe to call even if onUpdate was not previously Subscribed.
        /// </summary>
        public void Unsubscribe(Action<float> onUpdate)
        {
            _PendingHandlers.Enqueue(() =>
            {
                _SubscriberData.Remove(onUpdate);
            });
        }

        #region Fields

        private class SubscriberData
        {
            public float Period;
            public float NextCallTime;
            public float LastCallTime;
        }

        private readonly Queue<Action>                             _PendingHandlers = new ();
        private readonly Dictionary<Action<float>, SubscriberData> _SubscriberData  = new ();

        #endregion Fields

    }
}