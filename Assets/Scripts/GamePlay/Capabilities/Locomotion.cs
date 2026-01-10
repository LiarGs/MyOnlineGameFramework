using GamePlay.Configuration.Capabilities;
using GamePlay.Objects.Actors;
using UnityEngine;
using Utils;

namespace GamePlay.Capabilities
{
    public class Locomotion : CapabilityBase
    {
        #region UnityBehavior

        private void FixedUpdate()
        {
            if (PlanarVelocity.magnitude > 0.1f)
            {
                _HandleMove();
                _HandleRotation();
            }

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

        private void _HandleMove()
        {
            PlanarVelocity.y = 0;
            ActorBrain.ActorCharacterController.Move(PlanarVelocity * (Config.MoveSpeed * G.DeltaTime));
            // ActorBrain.AnimatorManager?.UpdateAnimatorMovementParameters(0, MoveAmount);
        }

        private void _HandleRotation()
        {
            var rotationDirection = transform.InverseTransformDirection(PlanarVelocity);
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