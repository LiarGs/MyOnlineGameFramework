using System.Collections;
using GamePlay.Action;
using UnityEngine;
using Utils;

namespace GamePlay.Actors
{
    public class ThirdPersonController : ActorControllerBase
    {
        #region PublicMethods

        public ThirdPersonController(Brain actorBrain) : base(actorBrain)
        {
            if (!_ActorBrain.IsOwner) return;
            _ActorBrain.StartCoroutine(_SetupCamera());
            G.UpdateRunner.Subscribe(_Tick, 0);
        }

        public override void Dispose()
        {
            G.UpdateRunner.Unsubscribe(_Tick);
            G.MainCamera.ActiveVirtualCamera.Follow = null;
            G.MainCamera.ActiveVirtualCamera.LookAt = null;
        }

        #endregion PublicMethods

        #region PrivateMethods

        private IEnumerator _SetupCamera()
        {
            // 这里等待一帧是为了让 ActiveVirtualCamera 初始化好
            yield return null;
            G.MainCamera.ActiveVirtualCamera.Follow = _ActorBrain.transform;
            G.MainCamera.ActiveVirtualCamera.LookAt = _ActorBrain.LookAtPos;
        }

        private void _Tick(float deltaTime)
        {
            _HandleMove();
        }

        private void _HandleMove()
        {
            var moveDirection = G.UserInput.VerticalInput * G.MainCamera.transform.forward;
            moveDirection += G.UserInput.HorizontalInput * G.MainCamera.transform.right;

            moveDirection = Quaternion.FromToRotation(Vector3.up, _ActorBrain.ActorCharacterController.transform.up) *
                            moveDirection;

            _ActorBrain.ExecuteCommand(new MoveCommand(moveDirection, G.UserInput.MoveAmount));
        }

        #endregion PrivateMethods
    }
}