using GamePlay.Actors;
using GamePlay.Capabilities;
using UnityEngine;

namespace GamePlay.Action
{
    public readonly struct MoveCommand : ICommand
    {
        #region PublicMethods

        public MoveCommand(Vector3 moveDirection, float moveAmount)
        {
            _MoveDirection = moveDirection;
            _MoveAmount    = moveAmount;
        }

        public void Execute(Brain executor)
        {
            var locomotion = executor.GetCapability<Locomotion>();
            if (!locomotion) return;

            locomotion.PlanarVelocity.x = _MoveDirection.x;
            locomotion.PlanarVelocity.z = _MoveDirection.z;
            locomotion.MoveAmount       = _MoveAmount;
        }

        #endregion PublicMethods

        #region Fields

        private readonly float   _MoveAmount;
        private readonly Vector3 _MoveDirection;

        #endregion Fields
    }
}