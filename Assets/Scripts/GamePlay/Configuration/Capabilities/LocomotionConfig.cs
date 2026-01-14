using UnityEngine;

namespace GamePlay.Configuration.Capabilities
{
    [CreateAssetMenu(fileName = "LocomotionConfig", menuName = "CapabilityConfig/LocomotionConfig")]
    public class LocomotionConfig : ScriptableObject
    {
        #region Fields

        public float MoveSpeed         = 4;
        public float RotationSpeed     = 15;
        public float GravityForce      = -10;
        public bool  ShouldMockGravity = true;

        #endregion Fields
    }
}