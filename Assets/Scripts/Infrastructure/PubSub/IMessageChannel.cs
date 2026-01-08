using System;

namespace Infrastructure.PubSub
{
    public interface IPublisher<T>
    {
        void Publish(T message);
    }

    public interface ISubscriber<T>
    {
        IDisposable Subscribe(Action<T>   handler);
        void        Unsubscribe(Action<T> handler);
    }

    public interface IMessageChannel<T> : IPublisher<T>, ISubscriber<T>, IDisposable
    {
        bool IsDisposed { get; }
    }
    
    // keep the last Message and publish it immediately when subscribe happen
    public interface IBufferedMessageChannel<T> : IMessageChannel<T>
    {
        bool HasBufferedMessage { get; }
        T    BufferedMessage    { get; }
    }
}