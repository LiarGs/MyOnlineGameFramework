using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using Utils.MultiPlayer;

namespace GamePlay.GameState
{
    public class DemoState : GameStateBehaviour
    {
        #region UnityBehavior

        protected void Awake()
        {
            _NetcodeHooks.OnNetworkSpawnHook   += _OnNetworkSpawn;
            _NetcodeHooks.OnNetworkDespawnHook += _OnNetworkDespawn;
        }

        #endregion UnityBehavior

        #region PrivateMethods

        private void _OnNetworkSpawn()
        {
            if (!G.NetworkManager.IsServer)
            {
                enabled = false;
                return;
            }

            G.NetworkManager.SceneManager.OnLoadEventCompleted  += _OnLoadEventCompleted;
            G.NetworkManager.SceneManager.OnSynchronizeComplete += _OnSynchronizeComplete;
        }

        private void _OnNetworkDespawn()
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted  -= _OnLoadEventCompleted;
            NetworkManager.Singleton.SceneManager.OnSynchronizeComplete -= _OnSynchronizeComplete;
        }

        private void _OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted,
            List<ulong>                           clientsTimedOut)
        {
            foreach (var clientId in clientsCompleted)
            {
                _SpawnPlayer(clientId);
            }
        }

        private void _OnSynchronizeComplete(ulong clientId)
        {
            //somebody joined after the initial spawn. This is a Late Join scenario. This player may have issues
            //(either because multiple people are late-joining at once, or because some dynamic entities are
            //getting spawned while joining. But that's not something we can fully address by changes in
            //ServerBossRoomState.
            _SpawnPlayer(clientId);
            Debug.Log("Synchronized client ID: " + clientId);
        }

        private void _SpawnPlayer(ulong clientId)
        {
            var spawnPosition  = _PlayerSpawnPoints[Random.Range(0, _PlayerSpawnPoints.Length)];
            var playerInstance = Instantiate(PlayerPrefab, spawnPosition.position, Quaternion.identity);

            playerInstance.SpawnAsPlayerObject(clientId);
        }

        #endregion PrivateMethods

        #region Porperties

        public override GameState ActiveState => GameState.Demo;

        #endregion Porperties

        #region Fields

        public NetworkObject PlayerPrefab;

        [SerializeField] private NetcodeHooks _NetcodeHooks;
        [SerializeField] private Transform[]  _PlayerSpawnPoints;

        #endregion Fields
    }
}