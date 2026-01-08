using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Infrastructure.PubSub
{
 public class MessageChannel<T> : IMessageChannel<T>
    {
        #region Interfaces

        public virtual void Publish(T message)
        {
            foreach (var handler in _PendingHandlers.Keys)
            {
                if (_PendingHandlers[handler])
                {
                    _MessageHandlers.Add(handler);
                }
                else
                {
                    _MessageHandlers.Remove(handler);
                }
            }
            _PendingHandlers.Clear();

            foreach (var messageHandler in _MessageHandlers)
            {
                if (messageHandler != null)
                {
                    messageHandler.Invoke(message);
                }
            }
        }

        public virtual IDisposable Subscribe(Action<T> handler)
        {
            Assert.IsTrue(!_IsSubscribed(handler), "Attempting to subscribe with the same handler more than once");

            if (_PendingHandlers.ContainsKey(handler))
            {
                if (!_PendingHandlers[handler])
                {
                    _PendingHandlers.Remove(handler);
                }
            }
            else
            {
                _PendingHandlers[handler] = true;
            }

            var subscription = new DisposableSubscription<T>(this, handler);
            return subscription;
        }

        public void Unsubscribe(Action<T> handler)
        {
            if (_IsSubscribed(handler))
            {
                if (_PendingHandlers.ContainsKey(handler))
                {
                    if (_PendingHandlers[handler])
                    {
                        _PendingHandlers.Remove(handler);
                    }
                }
                else
                {
                    _PendingHandlers[handler] = false;
                }
            }
        }
        
        public virtual void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;
                _MessageHandlers.Clear();
                _PendingHandlers.Clear();
            }
        }
        
        #endregion Interfaces

        #region PrivateMethods
        private bool _IsSubscribed(Action<T> handler)
        {
            var isPendingRemoval = _PendingHandlers.ContainsKey(handler) && !_PendingHandlers[handler];
            var isPendingAdding  = _PendingHandlers.ContainsKey(handler) && _PendingHandlers[handler];
            return _MessageHandlers.Contains(handler) && !isPendingRemoval || isPendingAdding;
        }

        #endregion PrivateMethods
        
        #region Fields
        public bool IsDisposed { get; private set; } = false;

        private readonly List<Action<T>> _MessageHandlers = new List<Action<T>>();

        /// This dictionary of handlers to be either added or removed is used to prevent problems from immediate
        /// modification of the list of subscribers. It could happen if one decides to unsubscribe in a message handler
        /// etc.A true value means this handler should be added, and a false one means it should be removed
        private readonly Dictionary<Action<T>, bool> _PendingHandlers = new Dictionary<Action<T>, bool>();
    
        #endregion Fields
    }
}