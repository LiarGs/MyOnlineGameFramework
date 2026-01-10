using UnityEngine.SceneManagement;
using Utils;

namespace ConnectionManagement.ConnectionStates
{
    /// <summary>
    /// Connection state corresponding to when the NetworkManager is shut down. From this state we can transition to the
    /// ClientConnecting sate, if starting as a client, or the StartingHost state, if starting as a host.
    /// </summary>
    class OfflineState : ConnectionStateBase
    {
        #region PublicMethods

        public override void Enter()
        {
            // _LobbyServiceFacade.EndTracking();
            G.NetworkManager.Shutdown();
            if (SceneManager.GetActiveScene().name != _MainMenuSceneName)
            {
                G.SceneLoaderWrapper.LoadScene(_MainMenuSceneName, useNetworkSceneManager: false);
            }
        }

        public override void Exit()
        {
        }

        public override void StartClientIP(string playerName, string ipaddress, int port)
        {
            var connectionMethod = new ConnectionMethodIP(ipaddress, (ushort)port, playerName);
            _ConnectionManager.ClientReconnecting.Configure(connectionMethod);
            _ConnectionManager.ChangeState(_ConnectionManager.ClientConnecting.Configure(connectionMethod));
        }

        public override void StartClientLobby(string playerName)
        {
            // var connectionMethod = new ConnectionMethodRelay(_LobbyServiceFacade, _LocalLobby, _ConnectionManager, _ProfileManager, playerName);
            // _ConnectionManager.ClientReconnecting.Configure(connectionMethod);
            // _ConnectionManager.ChangeState(_ConnectionManager.ClientConnecting.Configure(connectionMethod));
        }

        public override void StartHostIP(string playerName, string ipaddress, int port)
        {
            var connectionMethod = new ConnectionMethodIP(ipaddress, (ushort)port, playerName);
            _ConnectionManager.ChangeState(_ConnectionManager.StartingHost.Configure(connectionMethod));
        }

        public override void StartHostLobby(string playerName)
        {
            // var connectionMethod = new ConnectionMethodRelay(_LobbyServiceFacade, _LocalLobby, _ConnectionManager, _ProfileManager, playerName);
            // _ConnectionManager.ChangeState(_ConnectionManager.StartingHost.Configure(connectionMethod));
        }

        #endregion PublicMethods

        #region Fields

        private const string _MainMenuSceneName = "MainMenu";

        #endregion Fields
    }
}