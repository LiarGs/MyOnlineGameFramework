using GamePlay.Configuration.Capabilities;
using GamePlay.Objects.Actors;
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
            _HandleMoveServerRpc(PlanarVelocity, MoveAmount);
            _HandleRotationServerRpc(PlanarVelocity);

            if (Config.ShouldMockGravity)
            {
                _MockGravity();
            }
            else
            {
                _ResetVerticalVelocity();
            }
        }

        #endregion UnityBehavior

        #region PrivateMethods

        [ServerRpc]
        private void _HandleMoveServerRpc(Vector3 planarVelocity, float moveAmount)
        {
            planarVelocity.y = 0;
            ActorBrain.ActorCharacterController.Move(planarVelocity * (Config.MoveSpeed * G.DeltaTime));
            ActorBrain.AnimatorManager?.UpdateAnimatorMovementParameters(0, moveAmount);
        }

        [ServerRpc]
        private void _HandleRotationServerRpc(Vector3 planarVelocity)
        {
            var rotationDirection = transform.InverseTransformDirection(planarVelocity);
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
        public Vector3          VerticalVelocity;
        public float            MoveAmount;
        public LocomotionConfig Config;

        #endregion Fields
    }
}