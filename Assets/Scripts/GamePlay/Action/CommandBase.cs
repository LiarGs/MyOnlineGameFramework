using GamePlay.Objects.Actors;

namespace GamePlay.Action
{
    public abstract class CommandBase
    {
        public abstract void Execute(Brain executor);
    }
}