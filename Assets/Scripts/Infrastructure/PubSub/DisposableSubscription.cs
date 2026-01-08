using System;

namespace Infrastructure.PubSub
{
    /// <summary>
    /// This class is a handle to an active Message Channel subscription and when disposed it unsubscribes from said channel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisposableSubscription<T> : IDisposable
    {
        #region Interfaces

        public void Dispose()
        {
            if (!_IsDisposed)
            {
                _IsDisposed = true;

                if (!_MessageChannel.IsDisposed)
                {
                    _MessageChannel.Unsubscribe(_Handler);
                }

                _Handler        = null;
                _MessageChannel = null;
            }
        }

        #endregion Interfaces
        
        #region PublicMethods

        public DisposableSubscription(IMessageChannel<T> messageChannel, Action<T> handler)
        {
            _MessageChannel = messageChannel;
            _Handler        = handler;
        }

        #endregion PublicMethods

        #region Fields
        
        private bool               _IsDisposed;
        private Action<T>          _Handler;
        private IMessageChannel<T> _MessageChannel;

        #endregion Fields
  
    }
}