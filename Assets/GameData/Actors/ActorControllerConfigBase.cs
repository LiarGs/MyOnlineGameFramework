using GamePlay.Actors;
using UnityEngine;

namespace GameData.Actors
{
    public abstract class ActorControllerConfigBase : ScriptableObject
    {
        #region PublicMethods

        public abstract ActorControllerBase CreateActorController(Brain actorBrain);

        #endregion PublicMethods
    }
}