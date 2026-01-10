using Infrastructure.PubSub;
using Unity.Netcode;
using UnityEngine;
using Utils;
using Utils.SceneManagement;

namespace ConnectionManagement.ConnectionStates
{
    /// <summary>
    /// Connection state corresponding to a listening host. Handles incoming client connections. When shutting down or
    /// being timed out, transitions to the Offline state.
    /// </summary>
    class HostingState : OnlineStateBase
    {
        #region PublicMethods

        public override void Enter()
        {
            G.SceneLoaderWrapper.LoadScene("DebugTest", useNetworkSceneManager: true);
        }

        public override void Exit()
        {
        }

        public override void OnClientConnected(ulong clientId)
        {
        }

        public override void OnClientDisconnect(ulong clientId)
        {
            if (clientId != G.NetworkManager.LocalClientId)
            {
            }
        }

        public override void OnUserRequestedShutdown()
        {
            var reason = JsonUtility.ToJson(ConnectStatus.HostEndedSession);
            for (var i = G.NetworkManager.ConnectedClientsIds.Count - 1; i >= 0; i--)
            {
                var id = G.NetworkManager.ConnectedClientsIds[i];
                if (id != G.NetworkManager.LocalClientId)
                {
                    G.NetworkManager.DisconnectClient(id, reason);
                }
            }

            _ConnectionManager.ChangeState(_ConnectionManager.Offline);
        }

        public override void OnServerStopped()
        {
            _ConnectStatusPublisher.Publish(ConnectStatus.GenericDisconnect);
            _ConnectionManager.ChangeState(_ConnectionManager.Offline);
        }

        /// <summary>
        /// This logic plugs into the "ConnectionApprovalResponse" exposed by Netcode.NetworkManager. It is run every time a client connects to us.
        /// The complementary logic that runs when the client starts its connection can be found in ClientConnectingState.
        /// </summary>
        /// <remarks>
        /// Multiple things can be done here, some asynchronously. For example, it could authenticate your user against an auth service like UGS' auth service. It can
        /// also send custom messages to connecting users before they receive their connection result (this is useful to set status messages client side
        /// when connection is refused, for example).
        /// Note on authentication: It's usually harder to justify having authentication in a client hosted game's connection approval. Since the host can't be trusted,
        /// clients shouldn't send it private authentication tokens you'd usually send to a dedicated server.
        /// </remarks>
        /// <param name="request"> The initial request contains, among other things, binary data passed into StartClient. In our case, this is the client's GUID,
        /// which is a unique identifier for their install of the game that persists across app restarts.
        ///  <param name="response"> Our response to the approval process. In case of connection refusal with custom return message, we delay using the Pending field.
        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse                               response)
        {
        }

        #endregion PublicMethods

        #region PrivateMethods

        // private ConnectStatus _GetConnectStatus(ConnectionPayload connectionPayload)
        // {
        //     if (G.NetworkManager.ConnectedClientsIds.Count >= _ConnectionManager.MaxConnectedPlayers)
        //     {
        //         return ConnectStatus.ServerFull;
        //     }
        //
        //     if (connectionPayload.IsDebug != Debug.isDebugBuild)
        //     {
        //         return ConnectStatus.IncompatibleBuildType;
        //     }
        //
        //     return SessionManager<SessionPlayerData>.Instance.IsDuplicateConnection(connectionPayload.PlayerId) ?
        //         ConnectStatus.LoggedInAgain : ConnectStatus.Success;
        // }

        #endregion PrivateMethods

        #region Fields

        // used in ApprovalCheck. This is intended as a bit of light protection against DOS attacks that rely on sending silly big buffers of garbage.
        private const int _MaxConnectPayload = 1024;

        #endregion Fields
    }
}