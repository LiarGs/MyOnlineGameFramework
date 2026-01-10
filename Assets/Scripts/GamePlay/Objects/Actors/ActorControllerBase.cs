using System;

namespace GamePlay.Objects.Actors
{
    public abstract class ActorControllerBase : IDisposable
    {
        #region PublicMethods

        protected ActorControllerBase(Brain actorBrain)
        {
            ActorBrain = actorBrain;
        }

        public virtual void Dispose()
        {
        }

        #endregion PublicMethods

        #region Fields

        public Brain ActorBrain;

        #endregion Fields
    }
}