using UnityEngine;
using Utils;

namespace ConnectionManagement.ConnectionStates
{
    /// <summary>
    /// Connection state corresponding to a connected client. When being disconnected, transitions to the
    /// ClientReconnecting state if no reason is given, or to the Offline state.
    /// </summary>
    class ClientConnectedState : OnlineStateBase
    {
        #region PublicMethods
        
        public override void Enter()
        {
            // if (_LobbyServiceFacade.CurrentUnityLobby != null)
            // {
            //     _LobbyServiceFacade.BeginTracking();
            // }
        }

        public override void Exit() { }

        public override void OnClientDisconnect(ulong _)
        {
            var disconnectReason = G.NetworkManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason) ||
                disconnectReason == "Disconnected due to host shutting down.")
            {
                _ConnectStatusPublisher.Publish(ConnectStatus.Reconnecting);
                _ConnectionManager.ChangeState(_ConnectionManager.ClientReconnecting);
            }
            else
            {
                var connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                _ConnectStatusPublisher.Publish(connectStatus);
                _ConnectionManager.ChangeState(_ConnectionManager.Offline);
            }
        }
        
        #endregion PublicMethods
    }
}
