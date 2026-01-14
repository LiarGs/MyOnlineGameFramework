using System;

namespace GamePlay.Actors
{
    public abstract class ActorControllerBase : IDisposable
    {
        #region PublicMethods

        protected ActorControllerBase(Brain actorBrain)
        {
            _ActorBrain = actorBrain;
        }

        public abstract void Dispose();

        #endregion PublicMethods

        #region Fields

        protected readonly Brain _ActorBrain;

        #endregion Fields
    }
}