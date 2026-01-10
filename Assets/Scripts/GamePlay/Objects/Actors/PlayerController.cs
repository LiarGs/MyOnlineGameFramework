using GamePlay.Action;
using UnityEngine;
using Utils;

namespace GamePlay.Objects.Actors
{
    public class PlayerController : ActorControllerBase
    {
        #region PublicMeythods

        public PlayerController(Brain actorBrain) : base(actorBrain)
        {
            G.UpdateRunner.Subscribe(_Tick, 0);
        }

        public override void Dispose()
        {
            G.UpdateRunner.Unsubscribe(_Tick);
        }

        #endregion PublicMeythods

        #region PrivateMethods

        private void _Tick(float deltaTime)
        {
            var moveDirection = G.UserController.VerticalInput * G.CurrentCamera.transform.forward;
            moveDirection += G.UserController.HorizontalInput * G.CurrentCamera.transform.right;

            moveDirection = Quaternion.FromToRotation(Vector3.up, ActorBrain.ActorCharacterController.transform.up) *
                            moveDirection;

            _CachedMoveCommand.Reset(moveDirection, G.UserController.MoveAmount);
            ActorBrain.ExecuteCommand(_CachedMoveCommand);
        }

        #endregion PrivateMethods

        #region Fields

        private readonly MoveCommand _CachedMoveCommand = new MoveCommand(Vector3.zero, 0f);

        #endregion Fields
    }
}