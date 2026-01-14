using GamePlay.Actors;
using Unity.Netcode;
using UnityEngine;

namespace GamePlay.Managers
{
    public abstract class AnimatorManagerBase : NetworkBehaviour
    {
        #region PublicMethods

        #endregion PublicMethods

        #region Fields

        public Brain    ActorBrain;
        public Animator Animator;

        #endregion Fields
    }
}