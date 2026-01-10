using GamePlay.Capabilities;
using GamePlay.Objects.Actors;
using UnityEngine;

namespace GamePlay.Action
{
    public class MoveCommand : CommandBase
    {
        #region PublicMethods

        public MoveCommand(Vector3 moveDirection, float moveAmount)
        {
            _MoveDirection = moveDirection;
            _MoveAmount    = moveAmount;
        }

        public override void Execute(Brain executor)
        {
            var locomotion = executor.GetCapability<Locomotion>();
            if (locomotion)
            {
                locomotion.PlanarVelocity.x = _MoveDirection.x;
                locomotion.PlanarVelocity.z = _MoveDirection.z;
                locomotion.MoveAmount       = _MoveAmount;
            }
        }

        public void Reset(Vector3 moveDirection, float moveAmount)
        {
            _MoveDirection = moveDirection;
            _MoveAmount    = moveAmount;
        }

        #endregion PublicMethods

        #region Fields

        private float   _MoveAmount;
        private Vector3 _MoveDirection;

        #endregion Fields
    }
}