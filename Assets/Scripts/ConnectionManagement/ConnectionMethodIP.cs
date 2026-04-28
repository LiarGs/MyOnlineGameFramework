using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Utils;

namespace ConnectionManagement
{
    /// <summary>
    /// Simple IP connection setup with UTP
    /// </summary>
    class ConnectionMethodIP : ConnectionMethodBase
    {
        #region PublicMethods

        public ConnectionMethodIP(string ip, ushort port, string playerName) : base(playerName)
        {
            _Ipaddress = ip;
            _Port      = port;
        }

        public override Task SetupHostConnectionAsync()
        {
            SetConnectionPayload(GetPlayerId(),
                _PlayerName); // Need to set connection payload for host as well, as host is a client too
            var utp = (UnityTransport)G.NetworkManager.NetworkConfig.NetworkTransport;
            utp.SetConnectionData(_Ipaddress, _Port);

            return Task.CompletedTask;
        }

        public override Task SetupClientConnectionAsync()
        {
            Debug.LogError("Not implemented");

            return Task.CompletedTask;
        }

        public override Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync()
        {
            throw new System.NotImplementedException();
        }

        #endregion PublicMethods

        #region Fields

        private string _Ipaddress;
        private ushort _Port;

        #endregion Fields
    }
}