using Unity.Netcode;
using UnityEngine;

namespace Infrastructure.MultiPlayer
{
    /// <summary>
    /// NetworkBehaviour that represents a player connection and is the "Default Player Prefab" inside Netcode for
    /// GameObjects' (Netcode) NetworkManager. This NetworkBehaviour will contain several other NetworkBehaviours that
    /// should persist throughout the duration of this connection, meaning it will persist between scenes.
    /// </summary>
    /// <remarks>
    /// It is not necessary to explicitly mark this as a DontDestroyOnLoad object as Netcode will handle migrating this
    /// Player object between scene loads.
    /// </remarks>
    [RequireComponent(typeof(NetworkObject))]
    public class PersistentPlayer : NetworkBehaviour
    {
        #region UnityBehavior

        public override void OnNetworkSpawn()
        {
            gameObject.name = "PersistentPlayer: " + OwnerClientId;

            // Note that this is done here on OnNetworkSpawn in case this NetworkBehaviour's properties are accessed
            // when this element is added to the runtime collection. If this was done in OnEnable() there is a chance
            // that OwnerClientID could be its default value (0).
            _PersistentPlayerRuntimeCollectionBase.Add(this);
            if (IsServer)
            {
                // var sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
                // if (sessionPlayerData.HasValue)
                // {
                //     var playerData = sessionPlayerData.Value;
                //     m_NetworkNameState.Name.Value = playerData.PlayerName;
                //     if (playerData.HasCharacterSpawned)
                //     {
                //         m_NetworkAvatarGuidState.AvatarGuid.Value = playerData.AvatarNetworkGuid;
                //     }
                //     else
                //     {
                //         m_NetworkAvatarGuidState.SetRandomAvatar();
                //         playerData.AvatarNetworkGuid = m_NetworkAvatarGuidState.AvatarGuid.Value;
                //         SessionManager<SessionPlayerData>.Instance.SetPlayerData(OwnerClientId, playerData);
                //     }
                // }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemovePersistentPlayer();
        }

        public override void OnNetworkDespawn()
        {
            RemovePersistentPlayer();
        }

        #endregion UnityBehavior

        #region PublicMethod

        void RemovePersistentPlayer()
        {
            _PersistentPlayerRuntimeCollectionBase.Remove(this);
            // if (IsServer)
            // {
            //     var sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
            //     if (sessionPlayerData.HasValue)
            //     {
            //         var playerData = sessionPlayerData.Value;
            //         playerData.PlayerName        = m_NetworkNameState.Name.Value;
            //         playerData.AvatarNetworkGuid = m_NetworkAvatarGuidState.AvatarGuid.Value;
            //         SessionManager<SessionPlayerData>.Instance.SetPlayerData(OwnerClientId, playerData);
            //     }
            // }
        }

        #endregion PublicMethod

        #region PublicField

        [SerializeField] PersistentPlayerRuntimeCollectionBase _PersistentPlayerRuntimeCollectionBase;

        // [SerializeField] NetworkNameState _NetworkNameState;
        //
        // [SerializeField] NetworkAvatarGuidState _NetworkAvatarGuidState;
        //
        // public NetworkNameState NetworkNameState => _NetworkNameState;
        //
        // public NetworkAvatarGuidState NetworkAvatarGuidState => _NetworkAvatarGuidState;

        #endregion PublicField
    }
}