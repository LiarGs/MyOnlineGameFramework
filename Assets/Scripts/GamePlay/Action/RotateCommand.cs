using GamePlay.Actors;
using GamePlay.Capabilities;
using UnityEngine;

namespace GamePlay.Action
{
    public readonly struct RotateCommand : ICommand
    {
        #region PublicMethods

        public RotateCommand(Vector3 rotationDirection)
        {
            _RotationDirection = rotationDirection;
        }

        public void Execute(Brain executor)
        {
            var locomotion = executor.GetCapability<Locomotion>();
            if (!locomotion) return;

            locomotion.PlanarRotation = _RotationDirection;
        }

        #endregion PublicMethods

        #region Fields

        private readonly Vector3 _RotationDirection;

        #endregion Fields
    }
}