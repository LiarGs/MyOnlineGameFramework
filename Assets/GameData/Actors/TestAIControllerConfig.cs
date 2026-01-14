using GamePlay.Actors;
using UnityEngine;

namespace GameData.Actors
{
    [CreateAssetMenu(fileName = "TestAIController", menuName = "ActorController/TestAIController")]
    public class TestAIControllerConfig : ActorControllerConfigBase
    {
        public override ActorControllerBase CreateActorController(Brain actorBrain)
        {
            return new TestAIController(actorBrain);
        }
    }
}