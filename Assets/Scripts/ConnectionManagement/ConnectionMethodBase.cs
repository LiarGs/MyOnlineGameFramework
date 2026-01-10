using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Utils;

namespace ConnectionManagement
{
    public abstract class ConnectionMethodBase
    {
        #region PublicMethods

        /// <summary>
        /// Setup the host connection prior to starting the NetworkManager
        /// </summary>
        /// <returns></returns>
        public abstract Task SetupHostConnectionAsync();

        /// <summary>
        /// Setup the client connection prior to starting the NetworkManager
        /// </summary>
        /// <returns></returns>
        public abstract Task SetupClientConnectionAsync();

        /// <summary>
        /// Setup the client for reconnection prior to reconnecting
        /// </summary>
        /// <returns>
        /// success = true if succeeded in setting up reconnection, false if failed.
        /// shouldTryAgain = true if we should try again after failing, false if not.
        /// </returns>
        public abstract Task<(bool success, bool shouldTryAgain)> SetupClientReconnectionAsync();

        #endregion PublicMethods

        #region ProtectedMethods

        protected ConnectionMethodBase(string playerName)
        {
            _PlayerName = playerName;
        }

        protected void SetConnectionPayload(string playerId, string playerName)
        {
            var payload = JsonUtility.ToJson(new ConnectionPayload()
            {
                PlayerId   = playerId,
                PlayerName = playerName,
                IsDebug    = Debug.isDebugBuild
            });

            var payloadBytes = System.Text.Encoding.UTF8.GetBytes(payload);

            G.NetworkManager.NetworkConfig.ConnectionData = payloadBytes;
        }

        /// Using authentication, this makes sure your session is associated with your account and not your device. This means you could reconnect
        /// from a different device for example. A playerId is also a bit more permanent than player prefs. In a browser for example,
        /// player prefs can be cleared as easily as cookies.
        /// The forked flow here is for debug purposes and to make UGS optional in Boss Room. This way you can study the sample without
        /// setting up a UGS account. It's recommended to investigate your own initialization and IsSigned flows to see if you need
        /// those checks on your own and react accordingly. We offer here the option for offline access for debug purposes, but in your own game you
        /// might want to show an error popup and ask your player to connect to the internet.
        protected string GetPlayerId()
        {
            // TODO
            return "Dev_Test";
        }

        #endregion ProtectedMethods

        #region Fields

        protected readonly string _PlayerName;

        protected const string _DtlsConnType = "dtls";

        #endregion Fields
    }

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
            throw new System.NotImplementedException();
        }

        public override Task SetupClientConnectionAsync()
        {
            throw new System.NotImplementedException();
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