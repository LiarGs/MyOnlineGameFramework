using Cinemachine;
using GamePlay.Action;
using UnityEngine;
using Utils;

namespace GamePlay.Actors
{
    public class ThirdPersonController : ActorControllerBase
    {
        #region PublicMethods

        public ThirdPersonController(Brain actorBrain, GameObject thirdPersonCameraPrefab) : base(actorBrain)
        {
            _VirtualCameraPrefab = thirdPersonCameraPrefab;
        }

        public override void Init()
        {
            if (!_ActorBrain.IsOwner) return;

            _SetupCamera();
            G.UpdateRunner.Subscribe(_Tick, 0);
        }

        public override void Dispose()
        {
            if (!_ActorBrain.IsOwner) return;

            G.UpdateRunner.Unsubscribe(_Tick);
            Object.Destroy(_VirtualCamera.gameObject);
        }

        #endregion PublicMethods

        #region PrivateMethods

        private void _SetupCamera()
        {
            _VirtualCamera = Object.Instantiate(_VirtualCameraPrefab, G.MainCamera.transform.parent)
                .GetComponent<CinemachineVirtualCamera>();

            _VirtualCamera.Follow = _ActorBrain.transform;
            _VirtualCamera.LookAt = _ActorBrain.LookAtPos;
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
            _ActorBrain.ExecuteCommand(new RotateCommand(moveDirection));
        }

        #endregion PrivateMethods

        #region Fields

        private readonly GameObject               _VirtualCameraPrefab;
        private          CinemachineVirtualCamera _VirtualCamera;

        #endregion Fields
    }
}