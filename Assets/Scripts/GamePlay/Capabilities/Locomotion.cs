using GamePlay.Actors;
using GamePlay.Configuration.Capabilities;
using Unity.Netcode;
using UnityEngine;
using Utils;

namespace GamePlay.Capabilities
{
    public class Locomotion : CapabilityBase
    {
        #region UnityBehavior

        private void Update()
        {
            if (!ActorBrain.IsOwner) return;

            _HandleLocomotionServerRpc(PlanarVelocity, PlanarRotation, Config.ShouldMockGravity);
        }

        #endregion UnityBehavior

        #region PrivateMethods

        [ServerRpc]
        private void _HandleLocomotionServerRpc(Vector3 planarVelocity, Vector3 planarRotation, bool shouldMockGravity)
        {
            _HandleMove(planarVelocity);
            _HandleRotation(planarRotation);

            if (shouldMockGravity)
            {
                _MockGravity();
            }
            else
            {
                _ResetVerticalVelocity();
            }
        }

        private void _HandleMove(Vector3 planarVelocity)
        {
            planarVelocity.y = 0;
            ActorBrain.ActorCharacterController.Move(planarVelocity * (Config.MoveSpeed * G.DeltaTime));
        }

        private void _HandleRotation(Vector3 planarRotation)
        {
            var rotationDirection = transform.InverseTransformDirection(planarRotation);
            rotationDirection.y = 0;
            rotationDirection   = transform.TransformDirection(rotationDirection);

            if (rotationDirection == Vector3.zero)
            {
                rotationDirection = transform.forward;
            }

            var newRotation = Quaternion.LookRotation(rotationDirection, ActorTransform.up);

            ActorTransform.rotation =
                Quaternion.Slerp(ActorTransform.rotation, newRotation, Config.RotationSpeed * G.DeltaTime);
        }

        private void _MockGravity()
        {
            VerticalVelocity.y += Config.GravityForce * Time.deltaTime;
            ActorBrain.ActorCharacterController.Move(VerticalVelocity * G.DeltaTime);

            if (IsGrounded)
            {
                _ResetVerticalVelocity();
            }
        }

        private void _ResetVerticalVelocity() => VerticalVelocity.y = Config.GravityForce * Time.deltaTime;

        #endregion PrivateMethods

        #region Porperties

        private Transform ActorTransform => ActorBrain.ActorCharacterController.transform;

        private bool IsGrounded => ActorBrain.ActorCharacterController.isGrounded;

        #endregion Porperties

        #region Fields

        public Brain            ActorBrain;
        public Vector3          PlanarVelocity;
        public Vector3          PlanarRotation;
        public Vector3          VerticalVelocity;
        public float            MoveAmount;
        public LocomotionConfig Config;

        #endregion Fields
    }
}