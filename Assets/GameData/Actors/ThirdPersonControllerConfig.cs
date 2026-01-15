using GamePlay.Actors;
using UnityEngine;

namespace GameData.Actors
{
    [CreateAssetMenu(fileName = "ThirdPersonControllerConfig",
        menuName = "ActorController/ThirdPersonControllerConfig")]
    public class ThirdPersonControllerConfig : ActorControllerConfigBase
    {
        public override ActorControllerBase CreateActorController(Brain actorBrain)
        {
            return new ThirdPersonController(actorBrain, ThirdPersonCameraPrefab);
        }

        public GameObject ThirdPersonCameraPrefab;
    }
}