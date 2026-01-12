using GamePlay.Action;
using UnityEngine;
using Utils;

namespace GamePlay.Objects.Actors
{
    public class PlayerController : ActorControllerBase
    {
        #region PublicMethods

        public PlayerController(Brain actorBrain) : base(actorBrain)
        {
            if (!_ActorBrain.IsOwner) return;
            _SetupCamera();
            G.UpdateRunner.Subscribe(_Tick, 0);
        }

        public override void Dispose()
        {
            G.UpdateRunner.Unsubscribe(_Tick);
        }

        #endregion PublicMethods

        #region PrivateMethods

        private void _SetupCamera()
        {
            G.MainCamera.CineMachineVirtualCamera.Follow = _ActorBrain.transform;
            G.MainCamera.CineMachineVirtualCamera.LookAt = _ActorBrain.LookAtPos;
        }

        private void _Tick(float deltaTime)
        {
            _HandleMove();
        }

        private void _HandleMove()
        {
            var moveDirection = G.UserController.VerticalInput * G.MainCamera.transform.forward;
            moveDirection += G.UserController.HorizontalInput * G.MainCamera.transform.right;

            moveDirection = Quaternion.FromToRotation(Vector3.up, _ActorBrain.ActorCharacterController.transform.up) *
                            moveDirection;

            _ActorBrain.ExecuteCommand(new MoveCommand(moveDirection, G.UserController.MoveAmount));
        }

        #endregion PrivateMethods
    }
}