using GamePlay.Capabilities;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace GamePlay.Managers
{
    public class HumanoidAnimatorManager : AnimatorManagerBase
    {
        #region UnityBehavior

        private void Start()
        {
            _Locomotion = ActorBrain.GetCapability<Locomotion>();
        }

        private void Update()
        {
            if (!ActorBrain.IsOwner) return;

            if (_Locomotion)
            {
                _UpdateMovementParameters(0, _Locomotion.MoveAmount);
            }
        }

        #endregion UnityBehavior

        #region PrivateMethods

        private void _UpdateMovementParameters(float horizontal, float vertical)
        {
            Animator.SetFloat(HorizontalHash, horizontal, 0.1f, G.DeltaTime);
            Animator.SetFloat(VerticalHash,   vertical,   0.1f, G.DeltaTime);
        }

        #endregion PrivateMethods

        #region Fields

        private static readonly int        HorizontalHash = Animator.StringToHash("Horizontal");
        private static readonly int        VerticalHash   = Animator.StringToHash("Vertical");
        private                 Locomotion _Locomotion;

        #endregion Fields
    }
}