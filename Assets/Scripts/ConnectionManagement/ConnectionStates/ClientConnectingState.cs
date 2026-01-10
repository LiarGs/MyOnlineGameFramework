using System;
using System.Threading.Tasks;
using UnityEngine;
using Utils;

namespace ConnectionManagement.ConnectionStates
{
    /// <summary>
    /// Connection state corresponding to when a client is attempting to connect to a server. Starts the client when
    /// entering. If successful, transitions to the ClientConnected state. If not, transitions to the Offline state.
    /// </summary>
    class ClientConnectingState : OnlineStateBase
    {
        #region PublicMethods

        public ClientConnectingState Configure(ConnectionMethodBase baseConnectionMethod)
        {
            _ConnectionMethod = baseConnectionMethod;
            return this;
        }

        public override void Enter()
        {
            try
            {
                // NGO's StartClient launches everything
                if (!G.NetworkManager.StartClient())
                {
                    throw new Exception("NetworkManager StartClient failed");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error connecting client, see following exception");
                Debug.LogException(e);
                _StartingClientFailed();
                throw;
            }

// #pragma warning disable 4014
//             ConnectClientAsync();
// #pragma warning restore 4014
        }

        public override void Exit()
        {
        }

        public override void OnClientConnected(ulong _)
        {
            _ConnectStatusPublisher.Publish(ConnectStatus.Success);
            _ConnectionManager.ChangeState(_ConnectionManager.ClientConnected);
        }

        public override void OnClientDisconnect(ulong _)
        {
            // client ID is for sure ours here
            _StartingClientFailed();
        }

        internal async Task ConnectClientAsync()
        {
            try
            {
                // Setup NGO with current connection method
                await _ConnectionMethod.SetupClientConnectionAsync();

                // NGO's StartClient launches everything
                if (!G.NetworkManager.StartClient())
                {
                    throw new Exception("NetworkManager StartClient failed");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error connecting client, see following exception");
                Debug.LogException(e);
                _StartingClientFailed();
                throw;
            }
        }

        #endregion PublicMethods

        #region PrivateMethods

        private static void _StartingClientFailed()
        {
            var disconnectReason = G.NetworkManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                _ConnectStatusPublisher.Publish(ConnectStatus.StartClientFailed);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                _ConnectStatusPublisher.Publish(connectStatus);
            }

            _ConnectionManager.ChangeState(_ConnectionManager.Offline);
        }

        #endregion PrivateMethods

        #region Fields

        protected ConnectionMethodBase _ConnectionMethod;

        #endregion Fields
    }
}