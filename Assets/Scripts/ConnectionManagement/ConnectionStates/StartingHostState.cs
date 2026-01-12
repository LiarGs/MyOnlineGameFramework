using System;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace ConnectionManagement.ConnectionStates
{
    /// <summary>
    /// Connection state corresponding to a host starting up. Starts the host when entering the state. If successful,
    /// transitions to the Hosting state, if not, transitions back to the Offline state.
    /// </summary>
    class StartingHostState : OnlineStateBase
    {
        #region PublicMethods

        public StartingHostState Configure(ConnectionMethodBase baseConnectionMethod)
        {
            _ConnectionMethod = baseConnectionMethod;
            return this;
        }

        public override void Enter()
        {
            _StartHost();
        }

        public override void Exit()
        {
        }

        public override void OnServerStarted()
        {
            _ConnectStatusPublisher.Publish(ConnectStatus.Success);
            _ConnectionManager.ChangeState(_ConnectionManager.Hosting);
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse                               response)
        {
            var connectionData = request.Payload;
            var clientId       = request.ClientNetworkId;
            // This happens when starting as a host, before the end of the StartHost call. In that case, we simply approve ourselves.
            if (clientId == G.NetworkManager.LocalClientId)
            {
                var payload = System.Text.Encoding.UTF8.GetString(connectionData);
                // https://docs.unity3d.com/2020.2/Documentation/Manual/JSONSerialization.html
                var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

                // SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.PlayerId,
                //     new SessionPlayerData(clientId, connectionPayload.PlayerName, new NetworkGuid(), 0, true));

                response.Approved           = true;
                response.CreatePlayerObject = true;
            }
        }

        public override void OnServerStopped()
        {
            _StartHostFailed();
        }

        #endregion PublicMethods

        #region PrivateMethods

        private static void _StartHost()
        {
            try
            {
                // await _ConnectionMethod.SetupHostConnectionAsync();

                // NGO's StartHost launches everything
                if (!G.NetworkManager.StartHost())
                {
                    _StartHostFailed();
                }
            }
            catch (Exception)
            {
                _StartHostFailed();
                throw;
            }
        }

        private static void _StartHostFailed()
        {
            _ConnectStatusPublisher.Publish(ConnectStatus.StartHostFailed);
            _ConnectionManager.ChangeState(_ConnectionManager.Offline);
        }

        #endregion PrivateMethods

        #region Fields

        // [Inject] private LobbyServiceFacade   _LobbyServiceFacade;
        // [Inject] private LocalLobby           _LocalLobby;
        private ConnectionMethodBase _ConnectionMethod;

        #endregion Fields
    }
}