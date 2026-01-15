using Cinemachine;
using GameData.Actors;
using GamePlay.Action;
using UnityEngine;
using Utils;

namespace GamePlay.Actors
{
    public class FirstPersonController : ActorControllerBase
    {
        #region PublicMethods

        public FirstPersonController(Brain actorBrain, GameObject firstPersonCameraPrefab) :
            base(actorBrain)
        {
            _VirtualCameraPrefab = firstPersonCameraPrefab;
        }

        public override void Init()
        {
            if (!_ActorBrain.IsOwner) return;

            _SetupCamera();
            G.UpdateRunner.Subscribe(_Tick, 0);
        }

        public override void Dispose()
        {
            G.UpdateRunner.Unsubscribe(_Tick);
            Object.Destroy(_VirtualCamera.gameObject);
        }

        #endregion PublicMethods

        #region PrivateMethods

        private void _SetupCamera()
        {
            _VirtualCamera = Object.Instantiate(_VirtualCameraPrefab, _ActorBrain.LookAtPos)
                .GetComponent<CinemachineVirtualCamera>();
            _HorizontalSpeed = _VirtualCamera.GetCinemachineComponent<CinemachinePOV>().m_VerticalAxis.m_MaxSpeed;
            _VirtualCamera.transform.localPosition = Vector3.zero;
        }

        private void _Tick(float deltaTime)
        {
            _HandleMove();
            _HandleLook(deltaTime);
        }

        private void _HandleMove()
        {
            var moveDirection = G.UserInput.VerticalInput * G.MainCamera.transform.forward;
            moveDirection += G.UserInput.HorizontalInput * G.MainCamera.transform.right;

            moveDirection = Quaternion.FromToRotation(Vector3.up, _ActorBrain.ActorCharacterController.transform.up) *
                            moveDirection;

            _ActorBrain.ExecuteCommand(new MoveCommand(moveDirection, G.UserInput.MoveAmount));
        }

        private void _HandleLook(float deltaTime)
        {
            var rotation = Vector3.up * G.UserInput.CameraHorizontalInput * _HorizontalSpeed * deltaTime;

            _ActorBrain.transform.Rotate(rotation);
        }

        #endregion PrivateMethods

        #region Fields

        private readonly GameObject               _VirtualCameraPrefab;
        private          CinemachineVirtualCamera _VirtualCamera;
        private          float                    _HorizontalSpeed;

        #endregion Fields
    }
}