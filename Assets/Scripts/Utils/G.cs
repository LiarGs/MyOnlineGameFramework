using ApplicationLifecycle.Messages;
using CameraUtils;
using ConnectionManagement;
using GamePlay.UserInput;
using Infrastructure;
using Infrastructure.PubSub;
using Unity.Netcode;
using UnityEngine;
using Utils.SceneManagement;

namespace Utils
{
    // Service Locator 无法控制 Scope 
    public static class G
    {
        #region Fields

        public static MainCameraWrapper  MainCamera         => MainCameraWrapper.Instance;
        public static UserInput          UserInput          => UserInput.Instance;
        public static UpdateRunner       UpdateRunner       => UpdateRunner.Instance;
        public static SceneLoaderWrapper SceneLoaderWrapper => SceneLoaderWrapper.Instance;
        public static ConnectionManager  ConnectionManager  => ConnectionManager.Instance;
        public static NetworkManager     NetworkManager     => NetworkManager.Singleton;
        public static bool               UseUnscaledDeltaTime = false;
        public static float              DeltaTime => UseUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime;

        #endregion Fields

        public static class MessageChannels
        {
            public static readonly MessageChannel<QuitApplicationMessage> QuitMessageChannel          = new();
            public static readonly MessageChannel<ConnectStatus>          ConnectStatusMessageChannel = new();
        }
    }
}