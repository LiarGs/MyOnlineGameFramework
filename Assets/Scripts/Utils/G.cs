using ApplicationLifecycle.Messages;
using Infrastructure.PubSub;
using Unity.Netcode;

namespace Utils
{
    public static class G
    {
        #region Fields

        public static NetworkManager  NetworkManager => NetworkManager.Singleton;

        #endregion Fields

        public static class MessageChannels
        {
            public static readonly MessageChannel<QuitApplicationMessage> QuitMessageChannel =  new();
        }
    }
}