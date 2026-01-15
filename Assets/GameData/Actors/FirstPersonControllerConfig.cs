using GamePlay.Actors;
using UnityEngine;

namespace GameData.Actors
{
    [CreateAssetMenu(fileName = "FirstPersonControllerConfig",
        menuName = "ActorController/FirstPersonControllerConfig")]
    public class FirstPersonControllerConfig : ActorControllerConfigBase
    {
        public override ActorControllerBase CreateActorController(Brain actorBrain)
        {
            return new FirstPersonController(actorBrain, FirstPersonCameraPrefab);
        }

        #region Fields

        public GameObject FirstPersonCameraPrefab;

        #endregion Fields
    }
}