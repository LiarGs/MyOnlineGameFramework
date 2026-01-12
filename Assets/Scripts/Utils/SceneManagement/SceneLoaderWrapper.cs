using Unity.Netcode;
using UnityEngine.SceneManagement;

namespace Utils.SceneManagement
{
    /// <summary>
    /// Manages a loading screen by wrapping around scene management APIs. It loads scene using the SceneManager,
    /// or, on listening servers for which scene management is enabled, using the NetworkSceneManager and handles
    /// the starting and stopping of the loading screen.
    /// </summary>
    public class SceneLoaderWrapper : NetworkBehaviour
    {
        #region UnityBehavior

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void Start()
        {
            SceneManager.sceneLoaded       += _OnSceneLoaded;
            NetworkManager.OnServerStarted += _OnNetworkingSessionStarted;
            NetworkManager.OnClientStarted += _OnNetworkingSessionStarted;
            NetworkManager.OnServerStopped += _OnNetworkingSessionEnded;
            NetworkManager.OnClientStopped += _OnNetworkingSessionEnded;
        }

        public override void OnDestroy()
        {
            SceneManager.sceneLoaded -= _OnSceneLoaded;
            if (NetworkManager != null)
            {
                NetworkManager.OnServerStarted -= _OnNetworkingSessionStarted;
                NetworkManager.OnClientStarted -= _OnNetworkingSessionStarted;
                NetworkManager.OnServerStopped -= _OnNetworkingSessionEnded;
                NetworkManager.OnClientStopped -= _OnNetworkingSessionEnded;
            }

            base.OnDestroy();
        }

        #endregion UnityBehavior

        #region PublicMethods

        /// <summary>
        /// Loads a scene asynchronously using the specified loadSceneMode, with NetworkSceneManager if on a listening
        /// server with SceneManagement enabled, or SceneManager otherwise. If a scene is loaded via SceneManager, this
        /// method also triggers the start of the loading screen.
        /// </summary>
        /// <param name="sceneName">Name or path of the Scene to load.</param>
        /// <param name="useNetworkSceneManager">If true, uses NetworkSceneManager, else uses SceneManager</param>
        /// <param name="loadSceneMode">If LoadSceneMode.Single then all current Scenes will be unloaded before loading.</param>
        public void LoadScene(string sceneName, bool useNetworkSceneManager,
            LoadSceneMode            loadSceneMode = LoadSceneMode.Single)
        {
            if (useNetworkSceneManager)
            {
                if (IsSpawned && IsNetworkSceneManagementEnabled && !NetworkManager.ShutdownInProgress)
                {
                    if (NetworkManager.IsServer)
                    {
                        // If is active server and NetworkManager uses scene management, load scene using NetworkManager's SceneManager
                        NetworkManager.SceneManager.LoadScene(sceneName, loadSceneMode);
                    }
                }
            }
            else
            {
                // Load using SceneManager
                var loadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
                if (loadSceneMode == LoadSceneMode.Single)
                {
                    // _ClientLoadingScreen.StartLoadingScreen(sceneName);
                    // _LoadingProgressManager.LocalLoadOperation = loadOperation;
                }
            }
        }

        #endregion PublicMethods

        #region PrivateMethods

        private void _OnNetworkingSessionStarted()
        {
            // This prevents this to be called twice on a host, which receives both OnServerStarted and OnClientStarted callbacks
            if (!_IsInitialized)
            {
                if (IsNetworkSceneManagementEnabled)
                {
                    NetworkManager.SceneManager.OnSceneEvent += _OnSceneEvent;
                }

                _IsInitialized = true;
            }
        }

        private void _OnNetworkingSessionEnded(bool unused)
        {
            if (_IsInitialized)
            {
                if (IsNetworkSceneManagementEnabled)
                {
                    NetworkManager.SceneManager.OnSceneEvent -= _OnSceneEvent;
                }

                _IsInitialized = false;
            }
        }

        private void _OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (!IsSpawned || NetworkManager.ShutdownInProgress)
            {
                // _ClientLoadingScreen.StopLoadingScreen();
            }
        }

        private void _OnSceneEvent(SceneEvent sceneEvent)
        {
            switch (sceneEvent.SceneEventType)
            {
                case SceneEventType.Load: // Server told client to load a scene
                    // Only executes on client or host
                    if (NetworkManager.IsClient)
                    {
                        // Only start a new loading screen if scene loaded in Single mode, else simply update
                        if (sceneEvent.LoadSceneMode == LoadSceneMode.Single)
                        {
                            // _ClientLoadingScreen.StartLoadingScreen(sceneEvent.SceneName);
                            // _LoadingProgressManager.LocalLoadOperation = sceneEvent.AsyncOperation;
                        }
                        else
                        {
                            // _ClientLoadingScreen.UpdateLoadingScreen(sceneEvent.SceneName);
                            // _LoadingProgressManager.LocalLoadOperation = sceneEvent.AsyncOperation;
                        }
                    }

                    break;
                case SceneEventType.LoadEventCompleted: // Server told client that all clients finished loading a scene
                    // Only executes on client or host
                    if (NetworkManager.IsClient)
                    {
                        // _ClientLoadingScreen.StopLoadingScreen();
                    }

                    break;
                case SceneEventType.Synchronize: // Server told client to start synchronizing scenes
                {
                    // Only executes on client that is not the host
                    if (NetworkManager.IsClient && !NetworkManager.IsHost)
                    {
                        if (NetworkManager.SceneManager.ClientSynchronizationMode == LoadSceneMode.Single)
                        {
                            // If using the Single ClientSynchronizationMode, unload all currently loaded additive
                            // scenes. In this case, we want the client to only keep the same scenes loaded as the
                            // server. Netcode For GameObjects will automatically handle loading all the scenes that the
                            // server has loaded to the client during the synchronization process. If the server's main
                            // scene is different to the client's, it will start by loading that scene in single mode,
                            // unloading every additively loaded scene in the process. However, if the server's main
                            // scene is the same as the client's, it will not automatically unload additive scenes, so
                            // we do it manually here.
                            _UnloadAdditiveScenes();
                        }
                    }

                    break;
                }
                case SceneEventType.SynchronizeComplete: // Client told server that they finished synchronizing
                    // Only executes on server
                    if (NetworkManager.IsServer)
                    {
                        // Send client RPC to make sure the client stops the loading screen after the server handles what it needs to after the client finished synchronizing, for example character spawning done server side should still be hidden by loading screen.
                        _ClientStopLoadingScreenRpc(RpcTarget.Group(new[] { sceneEvent.ClientId }, RpcTargetUse.Temp));
                    }

                    break;
            }
        }

        private void _UnloadAdditiveScenes()
        {
            var activeScene = SceneManager.GetActiveScene();
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.isLoaded && scene != activeScene)
                {
                    SceneManager.UnloadSceneAsync(scene);
                }
            }
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void _ClientStopLoadingScreenRpc(RpcParams clientRpcParams = default)
        {
            // _ClientLoadingScreen.StopLoadingScreen();
        }

        #endregion PrivateMethods

        #region Properties

        private bool IsNetworkSceneManagementEnabled => NetworkManager != null && NetworkManager.SceneManager != null &&
                                                        NetworkManager.NetworkConfig.EnableSceneManagement;

        #endregion Properties

        #region Fields

        internal static SceneLoaderWrapper Instance;
        // [SerializeField] private ClientLoadingScreen _ClientLoadingScreen;
        //
        // [SerializeField] private LoadingProgressManager _LoadingProgressManager;

        private bool _IsInitialized;

        #endregion Fields
    }
}