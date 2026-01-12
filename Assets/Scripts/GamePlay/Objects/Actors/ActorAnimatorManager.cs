using UnityEngine;
using Utils;

namespace GamePlay.Objects.Actors
{
    public class ActorAnimatorManager
    {
        public ActorAnimatorManager(Animator animator)
        {
            _Animator = animator;
        }

        #region PublicMethods

        public void UpdateAnimatorMovementParameters(float horizontal, float vertical)
        {
            _Animator.SetFloat(HorizontalHash, horizontal, 0.1f, G.DeltaTime);
            _Animator.SetFloat(VerticalHash,   vertical,   0.1f, G.DeltaTime);
        }

        #endregion PublicMethods

        #region Fields

        private readonly        Animator _Animator;
        private static readonly int      HorizontalHash = Animator.StringToHash("Horizontal");
        private static readonly int      VerticalHash   = Animator.StringToHash("Vertical");

        #endregion Fields
    }
}