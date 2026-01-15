using System;
using ConnectionManagement.ConnectionStates;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace ConnectionManagement
{
    /// <summary>
    /// This state machine handles connection through the NetworkManager. It is responsible for listening to
    /// NetworkManger callbacks and other outside calls and redirecting them to the current ConnectionState object.
    /// </summary>
    public class ConnectionManager : MonoBehaviour
    {
        #region UnityBehavior

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            _CurrentState = Offline;

            G.NetworkManager.OnServerStarted            += _OnServerStarted;
            G.NetworkManager.ConnectionApprovalCallback += _ApprovalCheck;
            G.NetworkManager.OnClientConnectedCallback  += _OnClientConnectedCallback;
            G.NetworkManager.OnTransportFailure         += _OnTransportFailure;
            G.NetworkManager.OnServerStopped            += _OnServerStopped;
            G.NetworkManager.OnClientDisconnectCallback += _OnClientDisconnectCallback;
        }

        private void OnDestroy()
        {
            if (G.NetworkManager == null)
            {
                return;
            }

            G.NetworkManager.OnServerStarted            -= _OnServerStarted;
            G.NetworkManager.ConnectionApprovalCallback -= _ApprovalCheck;
            G.NetworkManager.OnClientConnectedCallback  -= _OnClientConnectedCallback;
            G.NetworkManager.OnTransportFailure         -= _OnTransportFailure;
            G.NetworkManager.OnServerStopped            -= _OnServerStopped;
            G.NetworkManager.OnClientDisconnectCallback -= _OnClientDisconnectCallback;
        }

        #endregion UnityBehavior

        internal void ChangeState(ConnectionStateBase nextState)
        {
            Debug.Log(
                $"{name}: Changed connection state from {_CurrentState.GetType().Name} to {nextState.GetType().Name}.");

            _CurrentState?.Exit();
            _CurrentState = nextState;
            _CurrentState.Enter();
        }

        #region PublicMethods

        public void StartClientLobby(string playerName)
        {
            _CurrentState.StartClientLobby(playerName);
        }

        public void StartClientIp(string playerName, string ipaddress, int port)
        {
            _CurrentState.StartClientIP(playerName, ipaddress, port);
        }

        public void StartHostLobby(string playerName)
        {
            _CurrentState.StartHostLobby(playerName);
        }

        public void StartHostIp(string playerName, string ipaddress, int port)
        {
            _CurrentState.StartHostIP(playerName, ipaddress, port);
        }

        public void RequestShutdown()
        {
            _CurrentState.OnUserRequestedShutdown();
        }

        #endregion PublicMethods

        #region PrivateMethods

        private void _OnClientDisconnectCallback(ulong clientId)
        {
            _CurrentState.OnClientDisconnect(clientId);
        }

        private void _OnClientConnectedCallback(ulong clientId)
        {
            _CurrentState.OnClientConnected(clientId);
        }

        private void _OnServerStarted()
        {
            _CurrentState.OnServerStarted();
        }

        private void _ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse                        response)
        {
            _CurrentState.ApprovalCheck(request, response);
        }

        private void _OnTransportFailure()
        {
            _CurrentState.OnTransportFailure();
        }

        // we don't need this parameter as the ConnectionState already carries the relevant information
        private void _OnServerStopped(bool _)
        {
            _CurrentState.OnServerStopped();
        }

        #endregion PrivateMethods

        #region Porperties

        public int NbReconnectAttempts => _NbReconnectAttempts;

        #endregion Porperties

        #region Fields

        internal static ConnectionManager Instance;
        public          int               MaxConnectedPlayers = 8;

        internal readonly OfflineState            Offline            = new OfflineState();
        internal readonly ClientConnectingState   ClientConnecting   = new ClientConnectingState();
        internal readonly ClientConnectedState    ClientConnected    = new ClientConnectedState();
        internal readonly ClientReconnectingState ClientReconnecting = new ClientReconnectingState();
        internal readonly StartingHostState       StartingHost       = new StartingHostState();
        internal readonly HostingState            Hosting            = new HostingState();

        private                  ConnectionStateBase _CurrentState;
        [SerializeField] private int                 _NbReconnectAttempts = 2;

        #endregion Fields
    }

    public enum ConnectStatus
    {
        Undefined,
        Success,                 //client successfully connected. This may also be a successful reconnect.
        ServerFull,              //can't join, server is already at capacity.
        LoggedInAgain,           //logged in on a separate client, causing this one to be kicked out.
        UserRequestedDisconnect, //Intentional Disconnect triggered by the user.
        GenericDisconnect,       //server disconnected, but no specific reason given.
        Reconnecting,            //client lost connection and is attempting to reconnect.
        IncompatibleBuildType,   //client build type is incompatible with server.
        HostEndedSession,        //host intentionally ended the session.
        StartHostFailed,         // server failed to bind
        StartClientFailed        // failed to connect to server and/or invalid network endpoint
    }

    public struct ReconnectMessage
    {
        public ReconnectMessage(int currentAttempt, int maxAttempt)
        {
            CurrentAttempt = currentAttempt;
            MaxAttempt     = maxAttempt;
        }

        public int CurrentAttempt;
        public int MaxAttempt;
    }

    public struct ConnectionEventMessage : INetworkSerializeByMemcpy
    {
        public ConnectStatus ConnectStatus;
        // public FixedPlayerName PlayerName;
    }

    [Serializable]
    public class ConnectionPayload
    {
        public string PlayerId;
        public string PlayerName;
        public bool   IsDebug;
    }
}